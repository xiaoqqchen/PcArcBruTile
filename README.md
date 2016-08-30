# PcArcBruTile 0.4.1

# 注意！：此项目停止维护，如有需要请移至ArcBruTile官网
在ArcGIS中快速加载中国的网络地图(暂时不支持10.3的版本，10.3.1可以使用)
经过多天的测试，终于推出了PcArcBruTile 0.4.1版本，赶紧卸载原来的插件，安装新的吧！([336的小伙伴看这里~](#336特别版)) Thanks @hyx @tm

1. 修复谷歌地图加载Bug(谷歌服务时好时坏，遇到不能加载的情况就忍忍吧~)
2. 采用多线程和内存缓存较大地优化了地图的加载

本程序只是在[ArcBruTile](https://arcbrutile.codeplex.com/)上做了稍微修改，使其能够加载中国的网络地图。我们不生成代码，我们只是代码的搬运工...
<p>安装包地址：https://github.com/xiaoqqchen/PcArcBruTile/blob/master/Soft/ArcBruTileSetup.msi?raw=true</p>
具体使用方法参考[我的博客](http://www.cnblogs.com/pengchen/p/4771288.html)

##使用步骤
1.下载安装包，双击直接安装，安装成功后显示“Registration succeeded”表示注册成功。

<img src="http://images2015.cnblogs.com/blog/364847/201508/364847-20150831140032653-1216018214.png"/>

2.打开ArcMap，右键，勾选ChinaMap工具条

<img src="http://images2015.cnblogs.com/blog/364847/201508/364847-20150831140033185-243190152.png"/>

3.在ArcMap中显示工具条，点击菜单就可以在ArcMap中显示网络地图了。

<img src="http://images2015.cnblogs.com/blog/364847/201508/364847-20150831140034372-2081082899.png"/>

4.OSM 和 谷歌地图最好使用VPN加速。
##示例图片
<p><img src="https://github.com/xiaoqqchen/PcArcBruTile/blob/master/Soft/1.png"/></p>

##336特别版
作为336的一员，肯定要为实验室单独开发一个版本。[下载地址](https://github.com/xiaoqqchen/PcArcBruTile/raw/master/Soft/ArcBruTileSetup_336.msi)

特别之处是我利用了实验室的面包机来共享和缓存切片...
######把homes这个文件夹设置为y盘，其他都一样。
![图片](Soft/截图.jpg)


