@echo off
rem author:zhaixd,datetime:2017.6.19
echo ***************** topshelf服务安装命令查看 *****************
set serviceName=GrapNewsServiceHost.exe
:: 进入服务安装目录
cd /d %~dp0
echo 当前目录：%cd%
:: 查看命令帮助
%serviceName% help
pause
exit