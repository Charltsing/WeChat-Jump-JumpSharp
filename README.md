# WeChat-Jump-JumpSharp

微信跳一跳辅助工具

1、安装ADB。   
   adb工具包最新2017Google官方版下载https://dl.google.com/android/repository/platform-tools-latest-windows.zip
   压缩包中已经附带。解压缩复制到C盘，确保adb.exe在C:\Android\SDK\platform-tools目录中。这个目录可以自己改。
   
2、打开手机的USB调试，具体自行百度。（一般是在手机版本号上点若干次，会出现提示）

3、将手机连接到电脑，运行ADBJump.exe，如果检测到手机型号，说明设置正确。

4、打开跳一跳小程序，右键点击要跳到的位置，小黑人会自动跳跃。

如果有其他问题，看下面原作者的代码。我只是在这个程序上加入了自动识别小黑人。识别位置在小黑人右下角红点处。如果没有出现红点，说明识别失败，请左键点击黑人位置，右键点击跳跃位置。


See the [原作者](http://www.cnblogs.com/dotnet-org-cn/p/8149693.html) 



*****************************

1、重新写了获取截图代码，提升截图的效率。
2、增加调试输出。


*****************************

如果右键不跳，请检查小黑人脚下是否有红点，如果没有红点用左键点一下再右键。
如果是小米手机或其他什么特别的手机，在USB调试下方有USB调试（安全设置）打开允许模拟点击

ADB不工作的请netstat -ano | findstr "5037"  查看是什么东西占用的
假如是：显示PID  4280 
再具体点 tasklist /fi "pid eq 4280"，显示 360mobileloader.exe
杀掉进程 运行 taskkill /pid 4280
然后测试ADB是否正常

本程序在1920*1080分辨率测试通过，其他手机请自行改代码。
https://github.com/Charltsing/WeChat-Jump-JumpSharp