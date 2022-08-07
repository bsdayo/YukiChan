﻿using System.Text.RegularExpressions;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;

namespace YukiChan.Utils;

public static class CommonUtils
{
    public static string[] ParseCommandBody(string body)
    {
        var args = body
            .Split(" ")
            .Where(elem => elem != "")
            .ToArray();

        YukiLogger.Debug($"Raw args: {string.Join(", ", args)}");

        return args;
    }

    public static (string[] args, string[] subFlags) ParseCommandBody(string body, string[] allSubFlags)
    {
        var args = ParseCommandBody(body);

        var subFlags = new List<string>();

        for (var i = 1; i <= args.Length; i++)
            if (allSubFlags.Contains(args[^i]))
                subFlags.Add(args[^i]);
            else break;

        args = args[..^subFlags.Count];

        YukiLogger.Debug($"Result args: {string.Join(", ", args)}");
        YukiLogger.Debug($"Sub flags: {string.Join(", ", subFlags)}");

        return (args, subFlags.ToArray());
    }

    public static MessageBuilder Reply(this MessageStruct message)
    {
        return new MessageBuilder()
            .Add(ReplyChain.Create(message));
    }

    public static MessageBuilder Reply(this MessageStruct message, string text)
    {
        return message.Reply().Text(text);
    }

    public static async Task Send(this Bot bot, MessageStruct message, string text)
    {
        await bot.Send(message, new MessageBuilder(text));
    }

    public static async Task Send(this Bot bot, MessageStruct message, MessageBuilder mb)
    {
        if (message.Type == MessageStruct.SourceType.Friend)
            await bot.SendFriendMessageWithLog(message.Sender.Name, message.Sender.Uin, mb);
        else if (message.Type == MessageStruct.SourceType.Group)
            await bot.SendGroupMessageWithLog(message.Receiver.Name, message.Receiver.Uin, mb);
    }

    public static async Task SendReply(this Bot bot, MessageStruct message, string text)
    {
        var mb = message.Reply(text);
        if (message.Type == MessageStruct.SourceType.Friend)
            await bot.SendFriendMessageWithLog(message.Sender.Name, message.Sender.Uin, mb);
        else if (message.Type == MessageStruct.SourceType.Group)
            await bot.SendGroupMessageWithLog(message.Receiver.Name, message.Receiver.Uin, mb);
    }

    public static double Bytes2MiB(this long bytes, int round)
    {
        return Math.Round(bytes / 1048576.0, round);
    }

    public static Dictionary<string, string> GetMetaData(this string html, params string[] keys)
    {
        var metaDict = new Dictionary<string, string>();

        foreach (var i in keys)
        {
            var pattern = i + @"=""(.*?)""(.|\s)*?content=""(.*?)"".*?>";

            // Match results
            foreach (Match j in Regex.Matches(html, pattern, RegexOptions.Multiline))
                metaDict.TryAdd(j.Groups[1].Value, j.Groups[3].Value);
        }

        return metaDict;
    }

    public static string FormatTimestamp(long timestamp, bool inMilliseconds = false)
    {
        return (inMilliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(timestamp)
                : DateTimeOffset.FromUnixTimeSeconds(timestamp))
            .LocalDateTime
            .ToString("yyyy.MM.dd HH:mm:ss");
    }
}

public class YukiException : Exception
{
    public YukiException(string message) : base(message)
    {
    }
}