@echo off
REM SchaleIzakaya Language Injector - GitHub 发布脚本
REM GitHub Repository Creation and Push Script for Windows

echo 🚀 SchaleIzakaya Language Injector - GitHub 发布脚本
echo ==================================================

REM 检查 Git 状态
echo 📋 检查 Git 状态...
git status

REM 添加所有文件
echo 📁 添加所有文件...
git add -A

REM 提交更改
echo 💾 提交更改...
git commit -m "feat: 完整发布 SchaleIzakaya Language Injector 文言文版本

- ✅ 完整的文言文翻译支持
- ✅ 多语言文件管理系统
- ✅ Harmony 补丁系统
- ✅ BepInEx 插件集成
- ✅ GitHub Actions 自动构建
- ✅ 完整的文档和贡献指南
- ✅ GPL v3 许可证

包含功能：
- 联动模块文本文言文化
- 地点描述文言文化
- 食物、饮料、食材名称翻译
- 动态文本替换系统
- 配置文件支持
- 调试日志系统"

REM 显示提交历史
echo 📜 提交历史:
git log --oneline -5

echo.
echo ✅ 本地仓库已准备完成！
echo.
echo 📋 下一步操作：
echo 1. 在 GitHub 上创建仓库: https://github.com/new
echo 2. 仓库名称: SchaleIzakaya-LanguageInjector
echo 3. 设置为公开仓库
echo 4. 添加 README 和 .gitignore (可选)
echo.
echo 🔗 然后运行以下命令推送代码：
echo git remote add origin https://github.com/GlassesMita/SchaleIzakaya-LanguageInjector.git
echo git push -u origin main
echo.
echo 📦 项目包含内容：
echo - 源代码 (C# 插件)
echo - 翻译文件 (文言文)
echo - 完整文档 (README, 贡献指南)
echo - GitHub Actions 工作流
echo - GPL v3 许可证
echo.
echo 🎉 感谢使用 SchaleIzakaya Language Injector！
echo.
pause