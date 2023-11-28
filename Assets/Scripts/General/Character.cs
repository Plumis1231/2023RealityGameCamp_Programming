using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    //最大血量
    public float maxHealth;
    //目前血量
    public float currentHealth;

    [Header("受伤无敌")]
    //无敌时间
    public float invulnerableDuration;
    //计时器
    private float invulnerableCounter;
    //状态
    public bool invulnerable;

    //创建一个事件控制血量变化，并传入到UI里
    public UnityEvent<Character> OnHealthChange;

    //创建一个受伤事件(中括号里面表示传入的transform这一组件)
    public UnityEvent<Transform> OnTakeDamage;
    //创建一个死亡事件
    public UnityEvent OnDie;

    private void Start()
    {
        //使得每次游戏开始时都有：满血状态
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {   
        //无敌时间递减
        invulnerableCounter -= Time.deltaTime;
        //当无敌时间过完之后，取消无敌状态
        if(invulnerableCounter <= 0){
            invulnerable = false;
        }
            
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Water"))
        {
            //死亡，更新血量，或者播放音效？
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();

        }
    }

    //接收伤害
    public void TakeDamage(Attack attacker)
    {
        //如果已经无敌，则不需要执行接受伤害的内容
        if(invulnerable)
            return;

        //如果血还能扣的话：执行减血以及触发无敌
        if(currentHealth - attacker.damage > 0 ){
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            //启动受伤事件
            OnTakeDamage?.Invoke(attacker.transform);

        }else{
            //触发死亡
            currentHealth = 0;
            //确认有注册的方式
            OnDie?.Invoke();
        }
        //血量减少的时候执行这个事件（括号里是传入一个character的值）
        OnHealthChange?.Invoke(this);
    }

        
        

    //创建“无敌触发器”函数方法
    private void TriggerInvulnerable(){
        //如果当前不是无敌状态，则改为无敌状态，并让计时器的数值等于无敌持续时间
        if(!invulnerable){
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

}