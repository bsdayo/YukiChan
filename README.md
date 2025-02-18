<div align="center">

## 暮雪酱

基于 [Flandre](https://github.com/FlandreDevs/Flandre) 框架开发的全新 v3 版本！

[![License](https://img.shields.io/github/license/b1acksoil/YukiChan?label=License&style=flat&color=7e57c2)](./LICENSE)
[![Contributors](https://img.shields.io/github/contributors/b1acksoil/YukiChan?label=Contributors&style=flat&color=1e88e5)](https://github.com/b1acksoil/YukiChan/graphs/contributors)
![.NET Version](https://img.shields.io/badge/.NET-7-1565c0?style=flat)  
[![bilibili](https://img.shields.io/badge/bilibili-暮雪酱__Official-f48fb1?style=flat)]()
[![QQ Group 1](https://img.shields.io/badge/QQ_一群-883632773-42a5fb?style=flat)]()
[![QQ Group 2](https://img.shields.io/badge/QQ_二群-744362693-42a5fb?style=flat)]()
[![QQ Guild](https://img.shields.io/badge/QQ_频道-e0r35nc9e2-00bcd4?style=flat)](https://qun.qq.com/qqweb/qunpro/share?_wv=3&_wwv=128&appChannel=share&inviteCode=11UIUD&businessType=9&from=246610&biz=ka)

\- **[使用文档](https://yukidocs.sorabs.cc/v3/)** \- **[用户协议](https://yukidocs.sorabs.cc/v3/policy.html)** \-

</div>

## 停止服务
自 2023 年 04 月 28 日起，暮雪酱已经停止服务。

> [暮雪酱停止服务声明]
> 
> 致各位暮雪酱用户：
> 
> 暮雪酱运营已有一年余久，得到了很多玩家的喜爱。我作为暮雪酱的开发与维护人员，衷心感谢你们的支持。
> 
> 但是在此，我怀着十分遗憾的心情宣布，暮雪酱计划无限期停止服务。暮雪酱本身为 Arcaea 查分所设计，既然查分功能已不复存在，那么暮雪酱本身也没有存在的意义了。这一次，暮雪可能真的要和各位说再见了。接下来，官群和频道也计划去除 Bot 属性，改为纯粹的音游交流社区。
> 
> 有关暮雪酱的开发历程，我写成了一篇文章，感兴趣的各位可以前往一看。我想说的话都在里面了。
> 
> [《一路走来，感谢有你 —— 暮雪酱 Arcaea 模块的前世今生》](https://bsdayo.moe/posts/memories-of-yukichan-arcaea-module/)
> 
> 再次对喜爱暮雪酱的玩家们，致以深深的歉意。
> 
> 谢谢大家。
> 
> bs  
> 2023.04.28

---

暮雪酱是一款完全公益性质的功能性聊天机器人，主要为广大 Arcaea 玩家提供查分服务，基于 .NET
7 + [Flandre](https://github.com/FlandreDevs/Flandre) 框架开发。

自 v3 开始，暮雪酱开始采用客户端-服务器架构，且以 AGPL v3.0 协议开源。客户端部分即本仓库，服务端部分在独立的[这个仓库](https://github.com/bsdayo/YukiChan.Server/)。

暮雪酱**暂时不支持自行搭建**，但暮雪酱 v3 的分布式架构已经为开放搭建做好了准备，如有兴趣可以关注我们的后续公告。

本仓库及源代码仅作交流学习使用。如果你对暮雪酱的相关代码实现有疑问、建议，欢迎 [提交 Issue](https://github.com/b1acksoil/YukiChan/issues)
或者 [发布 Discussion](https://github.com/b1acksoil/YukiChan/discussions) 讨论。v3 之前的旧版本不提供任何支持。

### 项目结构

本仓库包含以下项目：

- `YukiChan` - 客户端部分根项目，负责与 Flandre.Framework 交互。
- `YukiChan.Core` - 包含客户端共享部分
- `YukiChan.ImageGen` - 暮雪酱的图片生成模块
- `YukiChan.Client.Console` - 与暮雪酱服务器交互使用的 API 包装
- `YukiChan.Shared.Data` - 与暮雪酱服务器交互使用的通用请求/响应对象
- `YukiChan.Shared.Data.Console` - 与暮雪酱服务器交互使用的客户端部分请求/响应对象
- `YukiChan.Shared.Models` - 客户端与服务端共用的模型类
- `YukiChan.Shared.Utils` - 客户端与服务端共用的工具类
- `YukiChan.Tools` - **_等待重写_** 管理客户端使用的命令行工具

### 致谢

使用到的开源项目（按首字母排序）：

- [master131/BrotliSharpLib](https://github.com/master131/BrotliSharpLib)
- [FlandreDevs/Flandre](https://github.com/FlandreDevs/Flandre)
- [b1acksoil/Flandre.Plugins](https://github.com/b1acksoil/Flandre.Plugins)
- [beto-rodriguez/LiveCharts2](https://github.com/beto-rodriguez/LiveCharts2)
- [Microsoft.CodeAnalysis.CSharp.Scripting](https://github.com/dotnet/roslyn)
- [Microsoft.EntityFrameworkCore.Sqlite](https://github.com/dotnet/efcore)
- [serilog/serilog](https://github.com/serilog/serilog)
- [serilog/serilog-settings-configuration](https://github.com/serilog/serilog-settings-configuration)
- [serilog/serilog-sinks-console](https://github.com/serilog/serilog-sinks-console)
- [serilog/serilog-sinks-file](https://github.com/serilog/serilog-sinks-file)
- [mono/SkiaSharp](https://github.com/mono/SkiaSharp)
- [xoofx/Tomlyn](https://github.com/xoofx/Tomlyn)
- [bugproof/Tomlyn.Extensions.Configuration](https://github.com/bugproof/Tomlyn.Extensions.Configuration)
- [Marfusios/websocket-client](https://github.com/Marfusios/websocket-client)

以及，感谢所有支持暮雪酱开发的朋友，暮雪酱走到今天离不开你们的帮助。

### 开源协议
本仓库（即暮雪酱的客户端部分）采用 [AGPL v3](./LICENSE) 协议开源。
