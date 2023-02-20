# ECFSM
一个简单的适用于Unity的Entity，Component，FSM核心结构

# 结构：
Entity  
&emsp;└Com  
&emsp;&emsp;└Data - FSM  
Entiy统合Com，Com存储变量，Entity遍历Com执行Com的Data里的FSM，Fsm访问Com的数据，以及通过Entity访问其他Com的数据

# 概念：
在Com中使用<string, object>字典存储各种数据来避免创建一大堆类以及复杂的继承（当然，在状态机外完全可以使用继承，面向数据，面向对象混用思想）。使编码风格类似于脚本语言。

# 注意：
使用遍历前先通过获取字典里的值的指针来简化对字典内容的调用，这样避免经常使用字典编码的不便，所以状态机内部是Unsafe的。
