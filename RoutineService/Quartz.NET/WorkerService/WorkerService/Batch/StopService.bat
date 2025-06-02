@echo off
chcp 65001 >nul
set ServiceName=TestWorkerService

echo [INFO] 停止服務: %ServiceName%
sc stop %ServiceName%

if %errorlevel% equ 0 (
    echo [SUCCESS] 服務已停止。
) else (
    echo [ERROR] 無法停止服務或服務尚未執行。
)

pause
