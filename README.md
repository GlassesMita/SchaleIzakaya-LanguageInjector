# SchaleIzakaya Language Injector

东方夜雀食堂语言注入器 - 支持文言文翻译

## 🌟 功能特性

- ✅ **自定义语言注入** - 支持动态加载自定义翻译
- ✅ **文言文翻译** - 完整的文言文翻译支持
- ✅ **多语言文件管理** - 支持数百个翻译文件
- ✅ **动态文本替换** - 实时替换游戏内文本
- ✅ **BepInEx 插件集成** - 与 BepInEx 完美集成
- ✅ **Harmony 补丁系统** - 使用 HarmonyX 进行代码注入

## 📦 安装说明

### 前置要求
- [BepInEx](https://github.com/BepInEx/BepInEx) 已安装
- .NET 6.0 运行时

### 安装步骤
1. 下载最新版本的 `SchaleIzakaya.LanguageInjector.dll`
2. 将 DLL 文件复制到 `BepInEx/plugins/` 文件夹
3. 将翻译文件夹复制到 `BepInEx/plugins/SchaleIzakaya.LanguageInjector/`
4. 启动游戏

## ⚙️ 配置选项

插件配置文件位于 `BepInEx/config/` 文件夹，文件名为 `com.schale.languageinjector.cfg`

```ini
[General]
# 启用自定义语言注入
EnableCustomLanguage = true

# 自定义语言代码 (例如: zh-CN, zh-TW)
CustomLanguageCode = zh-CN

# 翻译文件路径
TranslationFilePath = ./BepInEx/plugins/SchaleIzakaya.LanguageInjector/zh_CN
```

## 📁 文件结构

```
BepInEx/plugins/SchaleIzakaya.LanguageInjector/
├── zh_CN/
│   ├── CollabModuleLang.txt     # 联动模块文本 (已文言文化)
│   ├── CommonPhrasesLang.txt    # 常用短语 (已文言文化)
│   ├── BeveragesLang.txt        # 饮料名称
│   ├── FoodsLang.txt            # 食物名称
│   ├── IngredientsLang.txt      # 食材名称
│   ├── DLC1_*.txt              # DLC1 翻译文件
│   ├── DLC2_*.txt              # DLC2 翻译文件
│   ├── DLC3_*.txt              # DLC3 翻译文件
│   ├── DLC4_*.txt              # DLC4 翻译文件
│   ├── DLC5_*.txt              # DLC5 翻译文件
│   └── ...                     # 其他翻译文件
└── translations.json           # 自定义翻译文件
```

## 🏗️ 开发指南

### 环境要求
- Visual Studio 2022 或更高版本
- .NET 6.0 SDK
- BepInEx 开发环境

### 编译步骤
```bash
# 克隆仓库
git clone https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector.git

# 进入项目目录
cd SchaleIzakaya-LanguageInjector

# 还原依赖
dotnet restore

# 编译项目
dotnet build
```

### 项目结构
```
SchaleIzakaya-LanguageInjector/
├── Models/                     # 数据模型
│   └── ModInfo.cs             # 模组信息
├── Patches/                    # Harmony 补丁
│   ├── CollabTextLoaderPatch.cs      # 联动文本加载补丁
│   ├── DataBaseLanguageGetFoodLangPatch.cs    # 食物文本补丁
│   ├── DataBaseLanguageGetBeverageLangPatch.cs  # 饮料文本补丁
│   ├── DataBaseLanguageGetIngredientLangPatch.cs # 食材文本补丁
│   ├── DialogTextLoaderPatch.cs      # 对话文本加载补丁
│   ├── LanguageTextLoaderPatch.cs    # 通用文本加载补丁
│   ├── PlayerSettingsMapLanguagePatch.cs  # 语言映射补丁
│   └── TextReplacementPatch.cs       # 文本替换补丁
├── Plugin.cs                  # 主插件文件
├── SchaleIzakaya.csproj       # 项目文件
└── README.md                  # 项目说明
```

## 🎯 文言文翻译示例

### 联动模块文本
```
原始文本: 欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！
文言翻译: 欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！

原始文本: 已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！
文言翻译: 已启矣！若有所需，亦可于此随时闭之！祝君游之畅！
```

### 地点描述
```
原始文本: 坐落在兽道的小树屋。原本只是个大一些的鸟巢，经过米斯蒂娅长年累月的拾掇，渐渐才有了家的模样。
文言翻译: 坐落兽道之小树屋。初本唯大鸟巢，经米斯蒂娅长年拾掇，渐成家貌。
```

## 🔧 技术细节

### 文本加载机制
插件使用多种 Harmony 补丁来拦截游戏的文本加载：

1. **语言映射补丁** (`PlayerSettingsMapLanguagePatch`)
   - 将自定义语言代码映射到游戏内部语言类型 5

2. **特定类型文本补丁**
   - `DataBaseLanguageGetFoodLangPatch` - 食物名称
   - `DataBaseLanguageGetBeverageLangPatch` - 饮料名称
   - `DataBaseLanguageGetIngredientLangPatch` - 食材名称

3. **通用文本替换补丁**
   - `TextReplacementPatch` - 直接替换目标文本
   - `CollabTextLoaderPatch` - 联动模块文本替换

### 文件格式
翻译文件使用制表符分隔格式：
```
ID	文本内容
```

### 编码支持
- UTF-8 编码
- 支持中文、日文、英文等多语言

## 🤝 贡献指南

欢迎提交 Issue 和 Pull Request！

### 提交新翻译
1. Fork 本仓库
2. 创建您的特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交您的更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 打开一个 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 🙏 致谢

- [BepInEx](https://github.com/BepInEx/BepInEx) - 模组框架
- [HarmonyX](https://github.com/BepInEx/HarmonyX) - 代码注入库
- 东方夜雀食堂开发团队

## 📞 联系方式

- GitHub Issues: [提交问题](https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector/issues)
- 项目主页: https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector

---

⭐ 如果这个项目对您有帮助，请给我们一个 Star！