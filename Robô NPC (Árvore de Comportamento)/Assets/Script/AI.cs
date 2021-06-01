using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;
using Unity.Collections.LowLevel.Unsafe;

namespace Panda.Examples.Shooter
{
    public class AI : MonoBehaviour
    {
        //Transform do player
        public Transform player; 
        //Selecionar o que vai spawnar balas
        public Transform bulletSpawn; 
        //Slider da vida
        public Slider healthBar;  
        //Prefab projetil
        public GameObject bulletPrefab; 

        //Pegar navmeshagent
        NavMeshAgent agent;
        //destino
        public Vector3 destination; 
        //Onde mirar
        public Vector3 target;     
        //vida do objeto
        float health = 100.0f; 
        //Velocidade de rotação
        float rotSpeed = 5.0f; 

        //Distancia da visao
        float visibleRange = 80.0f;
        //Distancia do tiro
        float shotRange = 40.0f;

        void Start()
        {
            //Colocando component
            agent = this.GetComponent<NavMeshAgent>(); 
            agent.stoppingDistance = shotRange - 5; //for a little buffer
            //Repete metodo que recupera vida
            InvokeRepeating("UpdateHealth", 5, 0.5f);
        }

        void Update()
        {
            //Vida acompanha camera 
            Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
            //Faz barra de vida ficar igual a vida do player
            healthBar.value = (int)health;
            //Posicao da barra de vida
            healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0); 
        }

        void UpdateHealth()
        {
            //Recupera 1 de vida se a vida ficar menos de 100
            if (health < 100)
                health++;
        }

        void OnCollisionEnter(Collision col)
        {
            //Perde 10 de vida se colidir com bala
            if (col.gameObject.tag == "bullet")
            {
                health -= 10;
            }
        }
        [Task]
        public void PickRandomDestination()
        {
            //Recebe vector 3 random em x e z que varia de -100 ate 100
            Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            //Faz movimento ate dest
            agent.SetDestination(dest);
            //Missao como sucesso
            Task.current.Succeed();
        }
        [Task]
        public void MoveToDestination()
        {
            //Tempo da missao
            if (Task.isInspected) Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
            //Se a distancia que resta for menor que a distancia dele parar, da a missao como bem sucedida
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                Task.current.Succeed(); 
            }
        }
        [Task]
        public void PickDestination(int x, int z)
        {
            //Marca o X e Y do destino
            Vector3 dest = new Vector3(x, 0, z);
            //Marca o destino
            agent.SetDestination(dest);
            //Da missao como bem sucedida
            Task.current.Succeed();

        }
        [Task]
        public void TargetPlayer()
        {
            //Marca target para transform position do player
            target = player.transform.position;
            //Da missao como bem sucedida
            Task.current.Succeed();
        }
        [Task]
        public bool Fire()
        {
            //Define instantiade da bala como um gameobject chamado bullet
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);

            //Addforce no rigidbody
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);

            return true;
        }
        [Task]
        public void LookAtTarget()
        {

            //Define direcao = target - transform.position
            Vector3 direction = target - this.transform.position;

            //Faz rotacao
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);


            if (Task.isInspected)
                Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));

            //Analisa angulo se for <5
            if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
            {
                //Da a missao como bem sucedida
                Task.current.Succeed();
            }    
        }
        [Task]
        bool SeePlayer()
        {

            Vector3 distance = player.transform.position - this.transform.position;
            //Cria raycast
            RaycastHit hit;
            //Define seewall falso
            bool seeWall = false;
            //Define parametros do drawray
            Debug.DrawRay(this.transform.position, distance, Color.red);
            //Se raycast colidir
            if(Physics.Raycast(this.transform.position, distance, out hit))
            {
                //Se colidir com a tag wall
                if (hit.collider.gameObject.tag == "wall")
                {
                    //Define seewall vardadeiro
                    seeWall = true;
                }
            }

            if(Task.isInspected)
                Task.current.debugInfo = string.Format("wall={0}", seeWall);

            if (distance.magnitude < visibleRange && !seeWall)
                return true;
            else
                return false;
        }
        [Task]
        bool Turn(float angle)
        {
            var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
            target = p;
            return true;
        }

        [Task]                                                 
        public bool IsHealthLessThan(float health)             
        {
            //Detecta se vida e menor que propria vida pra seguir outro processo
            return this.health < health;                      
        }

        [Task]                
        //Destruir gameobject
        public bool Explode()                  
        {
            //Destroi barra de vida
            Destroy(healthBar.gameObject);     
            //Destroi proprio gameobject
            Destroy(this.gameObject);          
            //Retorna verdadeiro
            return true;                       
        }
    }
}
