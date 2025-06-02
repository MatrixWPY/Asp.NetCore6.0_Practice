@echo off
chcp 65001 >nul
set ServiceName=TestWorkerService
set ExePath=D:\Publish\WorkerService\WorkerService.exe

echo [INFO] 正在建立 Windows Service: %ServiceName%

sc create %ServiceName% binPath= "%ExePath%" start= auto

if %errorlevel% equ 0 (
    echo [SUCCESS] 服務建立成功！
) else (
    echo [ERROR] 建立服務失敗！
)

pause
