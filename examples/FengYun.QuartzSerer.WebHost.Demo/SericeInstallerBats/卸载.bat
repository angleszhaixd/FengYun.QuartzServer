@echo off
rem author:zhaixd,datetime:2017.6.19
echo ******************** 服务卸载 ********************
set serviceName=GrapNewsServiceHost
:: 进入安装目录
cd /d %~dp0

  :: 服务停止
  :: %serviceName% stop
  sc stop %serviceName%
  if %ERRORLEVEL% ==0 (echo 服务停止 执行结果:√) else (echo 服务停止 执行结果:× %ERRORLEVEL%)  
  :: 服务删除
  :: %serviceName% uninstall  此方法删除必须重启电脑，才能再次安装服务,建议使用SC命令
  sc delete %serviceName%
  if %ERRORLEVEL% ==0 (echo 服务删除 执行结果:√) else (echo 服务删除 执行结果:× %ERRORLEVEL%)  
  
pause 
exit

