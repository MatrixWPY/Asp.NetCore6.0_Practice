@echo off
chcp 65001 >nul
set ServiceName=TestWorkerService

echo [INFO] 停止並刪除服務: %ServiceName%

sc stop %ServiceName%
sc delete %ServiceName%

if %errorlevel% equ 0 (
    echo [SUCCESS] 服務已刪除。
) else (
    echo [ERROR] 無法刪除服務（可能尚未建立或權限不足）。
)

pause
