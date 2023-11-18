using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //伤害值
    public int damage;
    //攻击范围
    public float attackRange;
    //攻击频率
    public float attackRate;

    //传递伤害
    private void OnTriggerStay2D(Collider2D other)
    {
        //问号，语法糖？：当对方存在该项的时候才会继续执行
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}
