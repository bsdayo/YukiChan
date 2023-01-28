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

</div>

暮雪酱是一款完全公益性质的功能性聊天机器人，主要为广大 Arcaea 玩家提供查分服务，基于 .NET
7 + [Flandre](https://github.com/FlandreDevs/Flandre) 框架开发。

自 v3 开始，暮雪酱开始采用客户端-服务器架构，客户端部分以 AGPL v3.0 协议开源（即本仓库），服务端部分暂时闭源。
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

使用到的开源项目：

- [master131/BrotliSharpLib](https://github.com/master131/BrotliSharpLib)
- [beto-rodriguez/LiveCharts2](https://github.com/beto-rodriguez/LiveCharts2)
- [FlandreDevs/Flandre](https://github.com/FlandreDevs/Flandre)
- [b1acksoil/Flandre.Plugins](https://github.com/b1acksoil/Flandre.Plugins)
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