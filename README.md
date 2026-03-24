# paramlab_cs

基于 Avalonia 的可视化参数编辑器

## 项目简介
paramlab_cs 是一个基于 .NET 8 和 Avalonia UI 跨平台开发的可视化参数编辑器，支持插件扩展、组件化管理、串口通信、OpenCV 图像处理等功能，适用于工程参数配置、可视化编辑和自动化场景。

## 主要功能
- 可视化参数编辑与管理
- 组件化架构，支持自定义扩展
- 串口通信配置与数据交互
- OpenCV 图像处理与显示
- 支持撤销/重做、文件管理、快捷键等编辑器功能

## 依赖环境
- .NET 8.0 及以上
- Avalonia 11.3.x
- OpenCvSharp4
- CommunityToolkit.Mvvm
- System.IO.Ports

## 安装与运行
1. 安装 .NET 8 SDK：https://dotnet.microsoft.com/download
2. 克隆本项目：
   ```bash
   git clone https://github.com/marling096/paramlab_cs.git
   ```
3. 还原依赖并编译：
   ```bash
   dotnet restore
   dotnet build
   ```
4. 运行：
   ```bash
   dotnet run --project paramlab_cs.csproj
   ```

## 目录结构
- `components/` 组件实现（串口、OpenCV、发送器等）
- `core/` 核心管理与事件系统
- `editor/` 编辑器主逻辑、文件管理、历史记录、快捷键等
- `assets/` 静态资源
- `App.axaml`/`MainWindow.axaml` 主界面与入口

## 快速上手
启动后可通过菜单栏进行新建、打开、保存等操作，主界面支持拖拽、参数编辑、组件交互等。

## 贡献
欢迎提交 issue 和 PR 参与改进！

## License
MIT
