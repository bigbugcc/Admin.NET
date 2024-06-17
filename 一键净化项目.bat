@echo OFF
 :begin
 REM 删除前端文件及文件夹
 DEL /f /s /q ".\Web\node_modules\*.*"
 RD /s /q ".\Web\node_modules"
 REM 循环删除指定文件夹下的文件夹
 FOR /d /r ".\Admin.NET\" %%b in (bin,obj,public) do rd /s /q "%%b"
 ECHO 【处理完毕，按任意键退出】
 PAUSE>NUL
 EXIT
 GOTO BEGIN