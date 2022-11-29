using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using EternalFS.Library.Commands;
using EternalFS.Library.Commands.Miscellaneous;
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

	[ByteSpan(" > ")]
	private static partial ReadOnlySpan<byte> WriteDelimiter();

	[ByteSpan("-mt")]
    private static partial ReadOnlySpan<byte> MeasureTime();

	static partial void PreprocessCommand(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> input, ref CommandExecutionResult? result)
	{
		context.ValueSpan = context.ValueSpan.SplitIndex(WriteDelimiter());

		int spaceIndex = input.IndexOf(ByteSpanHelper.Space());
		ReadOnlySpan<byte> commandSpan = input[(spaceIndex + 1)..];

		if (context.FileSystem is null &&
			CommandInfos.TryGetValue(commandSpan.GetString(), out var info) &&
			info.NeedsFileSystem)
			throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

		HandleHelpArgument(ref context, input, ref result);
		HandleMeasureTime(ref context, input);

		static void HandleHelpArgument(ref CommandExecutionContext context, in ReadOnlySpan<byte> input, ref CommandExecutionResult? result)
		{
			if (!context.ValueSpan.Contains(Help()))
				return;

			context.ValueSpan = input.SplitIndex(Help());
			result = ManCommand.Instance.Execute(ref context);
		}

        static void HandleMeasureTime(ref CommandExecutionContext context, in ReadOnlySpan<byte> input)
        {
            if (input.Contains(MeasureTime()))
            {
                Stopwatch stopwatch = new();
                stopwatch.Start();
				context.ServiceLocator.Add(stopwatch);
            }
        }
	}

	static partial void PostProcessCommand(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
	{
		HandleFileDelimiter(ref context, input);
		HandleMeasureTime(ref context);

		static void HandleFileDelimiter(ref CommandExecutionContext context, in ReadOnlySpan<byte> input)
		{
			ReadOnlySpan<byte> filename = input.SplitIndex(WriteDelimiter(), 1).SplitIndex(ByteSpanHelper.Space());

			if (filename == ReadOnlySpan<byte>.Empty)
				return;

			if (context.FileSystem is null)
				throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

			try
			{
				context.Accessor.LocateSubEntry(new(context.CurrentDirectory.FatEntryReference, filename));
			}
			catch (EternalFileSystemException e) when (e.State == EternalFileSystemState.CantLocateSubEntry)
			{
				context.Accessor.CreateSubEntry(new(context.CurrentDirectory.FatEntryReference, filename), false);
			}

            byte[] bytes = Encoding.UTF8.GetBytes(context.Writer.ToString());
            MemoryStream ms = new(bytes);

            context.Accessor.WriteFile(new(context.CurrentDirectory.FatEntryReference, filename), ms);

			context.Writer.Clear();
		}

        static void HandleMeasureTime(ref CommandExecutionContext context)
        {
            if (context.ServiceLocator.TryGet<Stopwatch>(out var stopwatch))
            {
				stopwatch.Stop();
                context.Writer.Append($"\nElapsed time (ticks): {stopwatch.ElapsedTicks}");
            }
        }
	}
}
