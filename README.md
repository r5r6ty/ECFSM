# ECFSM
一个简单的适用于Unity的Entity，Component，FSM游戏核心结构

# 结构：
Entity  
&emsp;└Com  
&emsp;&emsp;└Data - FSM  
&emsp;&emsp;&emsp;└游戏物体  
&emsp;&emsp;&emsp;&emsp;└游戏物体...  
Entiy统合Com，Com存储变量，Entity遍历Com执行Com里的Data的FSM，Fsm访问Com的数据，以及通过Entity访问其他Com的数据（跨Com访问数据还未实现）

# 概念：
在Com中使用<string, object>字典存储各种数据来避免创建一大堆类以及复杂的继承。使编码风格类似于脚本语言。  
在状态机外完全可以使用继承，面向数据、面向对象完全可以混用  
推荐使用能序列化字典的[Odin](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)（Odin可以序列化<string, object>字典，如果有免费能序列化这种字典的插件就好了）
内置Odin：[来源](https://github.com/Joker-YF/JKFrame/tree/main/JKFrame/Plugins/Sirenix)

# 注意：
使用变量前先通过获取字典里的值的指针来简化对字典内容的调用，这样避免经常使用字典编码的不便，所以状态机内部是Unsafe的。
