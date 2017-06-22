@echo off
rem author:zhaixd,datetime:2017.6.19
echo ******************** 服务安装 ********************
:: 设置应用程序名称，根据实际情况修改
set serviceName=GrapNewsServiceHost.exe
:: 进入服务安装目录
cd /d %~dp0
echo 当前目录：%cd%

::安装服务
  %serviceName% install --autostart
  if %ERRORLEVEL% ==0 (echo 服务安装 执行结果:√) else (echo 服务安装 执行结果:× %ERRORLEVEL%)
pause
exit