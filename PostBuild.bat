IF "%2" == "KK" (
	IF EXIST "C:\Illusion\Koikatu\" XCOPY /y "%1" "C:\Illusion\Koikatu\BepInEx\plugins\"
	IF EXIST "D:\Steam\steamapps\common\Koikatsu Party\BepInEx\" XCOPY /y "%1" "D:\Steam\steamapps\common\Koikatsu Party\BepInEx\plugins\KK_Plugins\"
	)
IF "%2" == "AI" (
	IF EXIST "D:\Illusion\AI-Syoujyo\" XCOPY /y "%1" "D:\Illusion\AI-Syoujyo\BepInEx\plugins\"
	)