@echo off
rem author:zhaixd,datetime:2017.6.19
echo ******************** 服务启动/停止/重启 ********************
set serviceName=GrapNewsServiceHost.exe
:: 进入安装目录
cd /d %~dp0
goto begin

rem 选择操作类型
:begin
set /p input=请选择操作类型(1=启动,2=停止,3=重启),按回车确认:
if %input% == 1 goto start
if %input% == 2 goto stop
if %input% == 3 goto reboot
if %input% neq 1 (if %input% neq 2 (if %input% neq 3 goto err))
pause
exit

rem 重启服务
:reboot
::服务重启
  %serviceName% stop
  if %ERRORLEVEL% ==0 (echo 服务停止 执行结果:√) else (echo 主服务停止 执行结果:× %ERRORLEVEL%)  
  %serviceName% start
  if %ERRORLEVEL% ==0 (echo 服务启动 执行结果:√) else (echo 主服务启动 执行结果:× %ERRORLEVEL%)  
  
pause 
exit


rem 停止服务
:stop
:: 服务停止
  %serviceName% stop
  if %ERRORLEVEL% ==0 (echo 服务停止 执行结果:√) else (echo 主服务停止 执行结果:× %ERRORLEVEL%)
pause 
exit


rem 启动服务
:start
  :: 服务启动
  %serviceName% start
  if %ERRORLEVEL% ==0 (echo 服务启动 执行结果:√) else (echo 主服务启动 执行结果:× %ERRORLEVEL%)  
pause 
exit

:err
echo × 输入不合法的序号,操作失败
set /p inputr=是否重新选择(y/n):
IF "%inputr%"=="y" goto begin
exit

