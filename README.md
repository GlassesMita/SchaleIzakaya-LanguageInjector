# SchaleIzakaya Language Injector

东方夜雀食堂语言注入器 - 支持文言文翻译

## 功能

- 支持自定义语言注入
- 文言文翻译支持
- 多语言文件管理
- 动态文本替换
- BepInEx 插件集成

## 安装

1. 确保已安装 BepInEx
2. 将编译好的 DLL 文件复制到 `BepInEx/plugins/` 文件夹
3. 启动游戏

## 配置

插件配置文件位于 `BepInEx/config/` 文件夹，可以设置：
- 启用/禁用自定义语言
- 自定义语言代码
- 翻译文件路径

## 文件结构

```
BepInEx/plugins/SchaleIzakaya.LanguageInjector/
├── zh_CN/
│   ├── CollabModuleLang.txt     # 联动模块文本
│   ├── CommonPhrasesLang.txt    # 常用短语
│   ├── BeveragesLang.txt        # 饮料名称
│   ├── FoodsLang.txt            # 食物名称
│   └── IngredientsLang.txt      # 食材名称
└── translations.json            # 自定义翻译文件
```

## 开发

使用 .NET 6.0 和 Visual Studio 2022 开发

### 编译
```bash
dotnet build
```

## 许可证

MIT License

## 作者

SchaleIzakaya 团队