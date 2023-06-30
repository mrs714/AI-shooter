
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cotxe : MonoBehaviour // MonoBehaviour és una classe de Unity. En aquest cas estem dient que Cotxe és una subclasse de MonoBehaviour
{
    // SerializeField fa que la variable aparegui a l'inspector de Unity
    [SerializeField] protected string marca;
    [SerializeField] protected string model;
    [SerializeField] protected string color;
    protected int velocitat; // velocitat a la que va
    public int velocitatMaxima; // velocitat màxima que pot arribar a assolir
    public int vida = 100; // pots definir el valor per defecte de la variable, o deixar-la sense valor
    public bool motorEnMarxa = false;
    protected Roda[] rodes = new Roda[4]; // [] vol dir que és un vector, i [4] que té 4 elements

    // constructor: serveix per crear noves instàncies de la classe, amb els paràmetres que li passem
    public Cotxe(int velocitat, int velocitatMaxima, string marca, string model, string color, int vida)
    {
        this.velocitat = velocitat;
        this.velocitatMaxima = velocitatMaxima;
        this.marca = marca;
        this.model = model;
        this.color = color;
        this.vida = vida;
    }

    // Start es crida quan s'executa el programa, tan sols una vegada
    protected void Start() 
    {
        // Aquí pots fer coses que només vols executar una vegada, com per exemple assignar un color aleatori al cotxe, o engegar-lo:

        motorEnMarxa = prepararCotxe(); // cridem a la funció prepararCotxe, que retorna un booleà (True/False, 1/0) que assignem a la variable 

    }

    virtual protected bool prepararCotxe() // El tipus de la funció és bool, perquè retorna un booleà, virtual ens permet fer override en una subclasse
    {
        // Aquí pots fer coses que només vols executar una vegada
        posarRodes(); // cridem a la funció posarRodes
        comprovarRodes(); // cridem a la funció comprovarRodes
        return true; // retorna true
    }
    

    // Update es crida cada frame, és a dir, 60 vegades per segon
    protected void Update()
    {
        // Aquí pots fer coses que vols que es repeteixin cada frame, com per exemple moure el cotxe, o fer que giri, o que acceleri
    }

    // Creem per exemple una nova classe, que només estarà disponible des de ScriptCotxe, roda
    protected class Roda // No volem que heredi de cap classe, per tant no posem : MonoBehaviour
    {
        public string tipus; // tipus de roda: hivern, estiu, pluja, muntanya
        public float friccio; // fricció de la roda, un decimal
        public int durabilitat; // durabilitat de la roda, un enter

        // Creem un constructor per a la classe Roda
        public Roda(string tipus = "estiu") 
        {

            // Les estructures condicionals són la base de la programació, i permeten distingir per casos. Són molt intuitives:
            if (tipus == "hivern") // si el tipus és hivern...
            {
                this.friccio = 0.8f;
                this.durabilitat = 100;
            }
            else if (tipus == "estiu") // sinó, si...
            {
                this.friccio = 0.9f;
                this.durabilitat = 80;
            }
            else if (tipus == "pluja")
            {
                this.friccio = 0.7f;
                this.durabilitat = 60;
            }
            else if (tipus == "muntanya")
            {
                this.friccio = 1.2f;
                this.durabilitat = 40;
            }
            else // sinó, si no és cap dels anteriors... (fixat que mai es donarà aquest cas si l'usuari no s'equivoca)
            {
                this.friccio = 0.5f;
                this.durabilitat = 20;
            }
        }
    }


    protected void posarRodes(string tipus = "estiu") // No volem que retorni res, per tant void, que vol dir buit, o "sense res", que és el que retorna: no res
    {
        for (int i = 0; i < rodes.Length; i++) 
        {
            rodes[i] = new Roda(tipus); // Assignem a la posició i del vector rodes una nova roda del tipus indicat
        }
    }

    // Ara, comprovem que les rodes s'hagin creat bé, mostrant la informació al terminal:
    protected void comprovarRodes()
    {
        foreach (Roda roda in rodes) 
        { 
            Debug.Log("Roda: " + roda.tipus + ", fricció: " + roda.friccio + ", durabilitat: " + roda.durabilitat);
        }
    }
}


// Per acabar, creem una subclasse de cotxe, CotxeMuntanya, que hereta de Cotxe.
// Això vol dir que CotxeMuntanya té totes les propietats i funcions de Cotxe, i a més, les seves pròpies.
public class CotxeMuntanya : Cotxe
{
    // Els cotxes de muntanya són més lents, i sempre porten rodes de muntanya.
    public CotxeMuntanya(int velocitat, int velocitatMaxima, string marca, string model, string color, int vida) : base(velocitat, velocitatMaxima, marca, model, color, vida) 
    {
        // Primer, s'executa el constructor de la superclasse amb els paràmetres. Després, ho podem modificar:
        this.velocitatMaxima = velocitatMaxima / 2; // Dividim la velocitat màxima per 2, perquè els cotxes de muntanya són més lents.
        this.vida = vida * 2; // Multipliquem la vida per 2, perquè els cotxes de muntanya són més resistents.
    }

    // Amb la resta de mètodes, els podem canviar per complet, amb override (sobreescriure), o afegir coses, amb new (nou).
    protected override bool prepararCotxe()
    {
        // Els cotxes de muntanya sempre porten rodes de muntanya:
        posarRodes("muntanya");
        return true;
    }

}