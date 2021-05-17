using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    //Variavel que pega transform do player
    public Transform player;
    //Variavel que pega transform do spawn da bullet
    public Transform bulletSpawn;
    //Variavel que pega slider de vida do player
    public Slider healthBar;
    //Variavel que pega prefab da bullet
    public GameObject bulletPrefab;

    //Variavel do agent
    NavMeshAgent agent;
    //Variavel do destino
    public Vector3 destination;
    //Variavel do alvo
    public Vector3 target;
    //Variavel vida do npc
    float health = 100.0f;
    //Varialvel velocidade de rotação
    float rotSpeed = 5.0f;

    //Variavel que passa raio de visibilidade
    float visibleRange = 80.0f;
    //Variavel que passa alcance de tiro
    float shotRange = 40.0f;

    void Start()
    {
        //Passa propriedades do NavMeshAgent para varivel agent
        agent = this.GetComponent<NavMeshAgent>();
        //for a little buffer
        agent.stoppingDistance = shotRange - 5;
        //Atualiza vida do npc a cada 5 segundos
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {
        //Passa posição da barra de vida do npc
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        //Aplica valor da variavel de vida ao slider
        healthBar.value = (int)health;
        //Posiciona barra de vida do npc
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    //Se vida for menor que 100 aumenta 1 na vida
    void UpdateHealth()
    {
       if(health < 100)
        health ++;
    }

    //Se colidir com bullet tira 10 da vida
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        //Variavel vector3 que passa localização aleatória
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        //Move agent para localização
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    //Debug do tempo em que o agent se move
    public void MoveToDestination()
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if(agent.remainingDistance >= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}

