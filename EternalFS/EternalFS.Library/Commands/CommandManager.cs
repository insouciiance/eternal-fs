using System;
using System.Diagnostics;
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

	static partial void PreprocessCommand(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> input, ref CommandExecutionResult? result)
	{
		context.ValueSpan = context.ValueSpan.SplitIndex(WriteDelimiter());

		int spaceIndex = input.IndexOf(ByteSpanHelper.Space());
		ReadOnlySpan<byte> commandSpan = input[(spaceIndex + 1)..];

		if (context.FileSystem is null &&
			CommandInfos.TryGetValue(commandSpan.GetString(), out var info) &&
			info.NeedsFileSystem)
			throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

		HandleHelpArgument(ref context, commandSpan, ref result);

		static void HandleHelpArgument(ref CommandExecutionContext context, in ReadOnlySpan<byte> commandSpan, ref CommandExecutionResult? result)
		{
			if (!context.ValueSpan.Contains(Help()))
				return;

			context.ValueSpan = commandSpan;
			result = ManCommand.Instance.Execute(ref context);
		}
	}

	static partial void PostProcessCommand(ref CommandExecutionContext context, scoped in ReadOnlySpan<byte> input, ref CommandExecutionResult result)
	{
		HandleFileDelimiter(ref context, input);

		static void HandleFileDelimiter(ref CommandExecutionContext context, in ReadOnlySpan<byte> input)
		{
			ReadOnlySpan<byte> filename = input.SplitIndex(WriteDelimiter(), 1).SplitIndex(ByteSpanHelper.Space());

			if (filename == ReadOnlySpan<byte>.Empty)
				return;

			if (!ValidationHelper.IsFilenameValid(filename))
				throw new EternalFileSystemException(EternalFileSystemState.InvalidFilename, filename.GetString());

			if (context.FileSystem is null)
				throw new CommandExecutionException(CommandExecutionState.MissingFileSystem);

			try
			{
				context.Accessor.LocateSubEntry(context.CurrentDirectory.FatEntryReference, filename);
			}
			catch (EternalFileSystemException e) when (e.State == EternalFileSystemState.CantLocateSubEntry)
			{
				context.Accessor.CreateSubEntry(context.CurrentDirectory.FatEntryReference, filename, false);
			}

			context.Accessor.WriteFile(context.CurrentDirectory.FatEntryReference, filename, Encoding.UTF8.GetBytes(context.Writer.ToString()));

			context.Writer.Clear();
		}
	}
}
