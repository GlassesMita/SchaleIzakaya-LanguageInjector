# SchaleIzakaya Language Injector 贡献指南

感谢您有兴趣为 SchaleIzakaya Language Injector 项目做出贡献！

## 🚀 快速开始

### 开发环境设置
1. 安装 [Visual Studio 2022](https://visualstudio.microsoft.com/) 或 [Visual Studio Code](https://code.visualstudio.com/)
2. 安装 [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
3. 克隆仓库：
   ```bash
   git clone https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector.git
   cd SchaleIzakaya-LanguageInjector
   ```

### 编译项目
```bash
dotnet restore
dotnet build
```

## 📝 贡献类型

### 1. 文言文翻译贡献
我们欢迎更多的文言文翻译！

#### 翻译指南：
- 保持文言文风格，使用古典汉语表达
- 避免现代白话文词汇
- 保持原文含义的准确性
- 使用适当的文言文虚词（之、乎、者、也等）

#### 示例翻译：
```
现代文: 欢迎访问《东方夜雀食堂》联动终端！
文言文: 欢迎访问《东方夜雀食堂》联运终端！

现代文: 祝您玩的开心！
文言文: 祝君游之畅！
```

### 2. 代码贡献
- 修复 bug
- 添加新功能
- 优化性能
- 改进文档

### 3. 测试贡献
- 测试不同游戏版本的兼容性
- 测试翻译文件的完整性
- 报告问题

## 🔄 工作流程

### 1. Fork 仓库
点击 GitHub 上的 "Fork" 按钮

### 2. 创建特性分支
```bash
git checkout -b feature/your-feature-name
```

### 3. 进行更改
- 修改代码或翻译文件
- 确保遵循项目编码规范
- 添加适当的注释

### 4. 测试更改
```bash
dotnet build
dotnet test  # 如果有测试项目
```

### 5. 提交更改
```bash
git add .
git commit -m "描述您的更改"
```

### 6. 推送到远程
```bash
git push origin feature/your-feature-name
```

### 7. 创建 Pull Request
在 GitHub 上创建 Pull Request，描述您的更改

## 📋 翻译文件格式

翻译文件使用制表符分隔格式：
```
ID	文本内容
```

### 示例：
```
0	欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！
1	已启矣！若有所需，亦可于此随时闭之！祝君游之畅！
```

## 🎯 翻译优先级

### 高优先级
- 主界面文本
- 游戏菜单
- 重要对话
- 教程文本

### 中优先级
- NPC 对话
- 物品描述
- 地点介绍

### 低优先级
- 成就文本
- 音乐标题
- 额外内容

## 🔧 开发规范

### 代码规范
- 使用有意义的变量名和函数名
- 添加适当的注释
- 遵循 C# 编码规范
- 保持代码整洁

### 提交信息规范
- 使用清晰、简洁的提交信息
- 格式：`类型: 描述`
- 示例：
  - `feat: 添加新的文言文翻译`
  - `fix: 修复文本加载问题`
  - `docs: 更新 README`

### 分支命名规范
- `feature/功能名称` - 新功能
- `fix/问题描述` - 修复问题
- `translation/翻译内容` - 翻译相关
- `docs/文档更新` - 文档更新

## 📚 文言文参考

### 常用词汇对照
| 现代文 | 文言文 |
|--------|--------|
| 欢迎 | 欢迎、惠临 |
| 访问 | 访问、造访 |
| 管理 | 管理、掌管 |
| 需要 | 所需、须 |
| 开心 | 畅、悦 |
| 游戏 | 游、戏 |
| 活动 | 活动、举 |
| 关闭 | 闭、阖 |
| 开启 | 启、开 |

### 虚词使用
- **之**：的、代词
- **乎**：吗、呢
- **者**：的人、的事物
- **也**：判断句末助词
- **矣**：了、已经
- **焉**：于之、在那里

## 🐛 报告问题

如果您发现问题，请通过以下方式报告：

1. 创建 GitHub Issue
2. 提供详细的问题描述
3. 包含复现步骤
4. 提供相关日志或截图

## 💬 联系我们

- GitHub Issues: [提交问题](https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector/issues)
- 项目主页: https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector

## 📄 许可证

本项目采用 GPL v3 许可证 - 查看 [LICENSE](../LICENSE) 文件了解详情

---

感谢您为 SchaleIzakaya Language Injector 项目做出贡献！ 🎉