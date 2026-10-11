# ECFSM
一个简单的适用于Unity的Entity，Component，FSM游戏核心结构

# 结构：
Entity
&emsp;└Com
&emsp;&emsp;└Data - FSM
&emsp;&emsp;&emsp;└游戏物体
&emsp;&emsp;&emsp;&emsp;└游戏物体...
Entity统合Com，Com存储变量，Entity遍历Com执行Com里的Data的FSM，Fsm访问Com的数据，以及通过Entity访问其他Com的数据（跨Com访问数据还未实现）

# 概念：
在Com中使用<string, object>字典存储各种数据来避免创建一大堆类以及复杂的继承。使编码风格类似于脚本语言。
在状态机外完全可以使用继承，面向数据、面向对象完全可以混用
推荐使用能序列化字典的[Odin](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)（Odin可以序列化<string, object>字典，如果有免费能序列化这种字典的插件就好了）
内置Odin：[来源](https://github.com/Joker-YF/JKFrame/tree/main/JKFrame/Plugins/Sirenix)

# 用法：
变量字典的值类型用 SetRef/GetRef 拿 ref 直接读写，改的就是字典里存的那份；引用类型用 Set/Get。

```csharp
// 值类型：拿到 ref 后当普通局部变量用
_V.SetRef("_speed", 0.0f);
ref float _speed = ref _V.GetRef<float>("_speed");
_speed += 1.0f;

// 引用类型：直接拿对象
_V.Set("boxCollider", gameObject.AddComponent<BoxCollider>());
_V.Get("animator", out Animator animator);
```

ref 返回值让「拿到变量就直改字典」这个手感保留了下来，同时全程内存安全：不再 pin 住 GC 对象、不再保存裸指针，也就不存在指针被 GC 释放后崩溃的一类问题，整个框架不需要 unsafe。注意拿到 ref 后别把对应的键 Del 掉（删了之后写 ref 会写进已移除的箱子里）。

# 注意：
SetRef/GetRef 的键不存在时会抛 KeyNotFoundException（带键名），先 Set 再 Get，别拼错键名。
Demo（Data/Testman）的状态在 Data 的 OnEnable 里注册，编辑器和真机都会执行。
