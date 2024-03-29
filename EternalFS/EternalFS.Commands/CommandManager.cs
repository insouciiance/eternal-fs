﻿using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using EternalFS.Commands;
using EternalFS.Commands.Diagnostics;
using EternalFS.Commands.Extensions;
using EternalFS.Commands.IO;
using EternalFS.Commands.Miscellaneous;
using EternalFS.Commands.Utils;
using EternalFS.Library.Diagnostics;
using EternalFS.Library.Extensions;
using EternalFS.Library.Filesystem;
using EternalFS.Library.Utils;

// ReSharper disable once CheckNamespace
/// <summary>
/// Handles command execution and acts as a centralized entry point to execute a command.
/// </summary>
/// <remarks>
/// <para>
/// A command is executed using <see cref="ExecuteCommand"/> from an input stream and <see cref="CommandExecutionContext"/>.
/// </para>
/// <para>
/// A user may add partial <see cref="PreprocessCommand"/> and <see cref="PostProcessCommand"/> methods that leverage the behaviour of
/// the execution before and after the command is executed respectively.
/// </para>
/// </remarks>
public static partial class CommandManager
{
    [ByteSpan("--help")]
    private static partial ReadOnlySpan<byte> Help();

    [ByteSpan(">")]
    private static partial ReadOnlySpan<byte> WriteDelimiter();

    [ByteSpan(">>")]
    private static partial ReadOnlySpan<byte> AppendDelimiter();

    [ByteSpan("-mt")]
    private static partial ReadOnlySpan<byte> MeasureTime();

    static partial void PreprocessCommand(ref CommandExecutionContext context, ref CommandExecutionResult? result)
    {
        if (context.FileSystem is null &&
            CommandInfos.TryGetValue(context.Reader.ReadCommandName().GetString(), out var info) &&
            info.NeedsFileSystem)
            throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

        HandleHelpArgument(ref context, ref result);
        HandleMeasureTime(ref context);
        HandleFileDelimiter(ref context);

        static void HandleHelpArgument(ref CommandExecutionContext context, ref CommandExecutionResult? result)
        {
            if (!context.Reader.TryReadNamedArgument(Help(), out _))
                return;

            ReadOnlySpan<byte> commandName = context.Reader.ReadCommandName();

            string manCommandName = CommandHelper.GetInfo<ManCommand>().Name;

            byte[] manCommand = ArrayPool<byte>.Shared.Rent(
                commandName.Length +
                ByteSpanHelper.Space().Length +
                manCommandName.Length);

            Encoding.UTF8.GetBytes(manCommandName).AsSpan().CopyTo(manCommand);
            ByteSpanHelper.Space().CopyTo(manCommand.AsSpan()[manCommandName.Length..]);
            commandName.CopyTo(manCommand.AsSpan()[(manCommandName.Length + ByteSpanHelper.Space().Length)..]);

            MemoryStream stream = new(manCommand);

            result = ExecuteCommand(stream, ref context);

            ArrayPool<byte>.Shared.Return(manCommand);
        }

        static void HandleMeasureTime(ref CommandExecutionContext context)
        {
            if (context.Reader.TryReadNamedArgument(MeasureTime(), out _))
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
                context.ServiceLocator.Add(stopwatch);
            }
        }

        static void HandleFileDelimiter(ref CommandExecutionContext context)
        {
            if (context.Reader.TryReadNamedArgument(WriteDelimiter(), out var writeArgument))
            {
                ReadOnlySpan<byte> filename = writeArgument.Value;
                RedirectToFile(ref context, filename, false);
                return;
            }

            if (context.Reader.TryReadNamedArgument(AppendDelimiter(), out var appendArgument))
            {
                ReadOnlySpan<byte> filename = appendArgument.Value;
                RedirectToFile(ref context, filename, true);
            }

            void RedirectToFile(ref CommandExecutionContext context, scoped ReadOnlySpan<byte> filename, bool append)
            {
                if (context.FileSystem is null)
                    throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

                SubEntryInfo info = new(context.CurrentDirectory.FatEntryReference, filename);

                try
                {
                    context.Accessor.LocateSubEntry(info);
                }
                catch (EternalFileSystemException e) when (e.State == EternalFileSystemState.CantLocateSubEntry)
                {
                    context.Accessor.CreateSubEntry(info, false);
                }

                context.ServiceLocator.Add(context.Writer);
                context.Writer = new FileEntryOutputWriter(info, context.Accessor, append);
            }
        }
    }

    static partial void PostProcessCommand(ref CommandExecutionContext context, ref CommandExecutionResult result)
    {
        HandleMeasureTime(ref context);

        if (!context.Reader.IsFullyRead)
            context.Writer.Warning("There were unrecognized parts in the command.");

        if (context.ServiceLocator.TryGet<IOutputWriter>(out var writer))
        {
            context.Writer.Flush();
            context.Writer = writer;
        }

        static void HandleMeasureTime(ref CommandExecutionContext context)
        {
            if (context.ServiceLocator.TryGet<Stopwatch>(out var stopwatch))
            {
                stopwatch.Stop();
                context.Writer.Info($"\nElapsed time (ticks): {stopwatch.ElapsedTicks}");
            }
        }
    }
}
