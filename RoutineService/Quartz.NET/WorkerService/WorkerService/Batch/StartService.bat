@echo off
chcp 65001 >nul
set ServiceName=TestWorkerService

echo [INFO] 啟動服務: %ServiceName%
sc start %ServiceName%

if %errorlevel% equ 0 (
    echo [SUCCESS] 服務啟動成功！
) else (
    echo [ERROR] 啟動服務失敗！
)

pause
