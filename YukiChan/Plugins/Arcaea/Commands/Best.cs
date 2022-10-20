// using Flandre.Core.Attributes;
// using Flandre.Core.Common;
// using Flandre.Core.Messaging;
//
// // ReSharper disable CheckNamespace
//
// namespace YukiChan.Plugins.Arcaea;
//
// public partial class ArcaeaPlugin
// {
//     [Command("best <songname: string>")]
//     public async Task<MessageContent> OnBest(MessageContext ctx, ParsedArgs args)
//     {
//         var id = await ArcaeaSongDatabase.FuzzySearchId(
//             args.GetArgument<string>("songname"));
//         // var best = await _auaClient.User.Best()
//     }
// }