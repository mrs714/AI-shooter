// els comentaris de una sola línia comencen amb doble barra
/* els comentaris de múltiples línies 
                                        comencen amb barra asterisc */

// using System fa que el codi pugui accedir a les classes del namespace System. Un namespace és un conjunt de classes, en aquest cas totes les de System.
// Ho pots veure com una llibreria, que conté un conjunt de llibreries més petites.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cotxe : MonoBehaviour // MonoBehaviour és una classe de Unity. En aquest cas estem dient que Cotxe és una subclasse de MonoBehaviour
{

    // SerializeField fa que la variable aparegui a l'inspector de Unity
    [SerializeField] string marca;
    [SerializeField] string model;
    [SerializeField] string color;
    private int velocitat; // velocitat a la que va
    public int velocitatMaxima; // velocitat màxima que pot arribar a assolir
    public int vida = 100; // pots definir el valor per defecte de la variable, o deixar-la sense valor
    public bool motorEnMarxa = false;

    /* les variables normalment són privades, només s'hi pot accedir des del mateix "scope" (àmbit, en aquest cas des de la classe), 
    però si volem que es puguin veure des de fora, les fem públiques. També pots escriure private enlloc de públic,
    però si no posses res es sobreenten. Les classes també poden ser públiques o privades, però com que volem instanciar-les (fer còpies) 
    des de altres scripts, les fem públiques. Hi ha diferència entre utilitzar SerializeField i fer una variable pública. Si és pública, 
    pots llegir però també modificar el valor des de fora la classe i des de l'editor. En canvi, amb SerializeField, només es pot fer al editor de Unity.
    Un altre tipus de paràmetre és "protected", que vol dir que és privat, però les sub-classes poden accedir (hi ha altres paràmetres però amb aquests en tenim prou)*/


    // constructor: serveix per crear noves instàncies de la classe, amb els paràmetres que li passem
    public Cotxe(int velocitat, int velocitatMaxima, string marca, string model, string color, int vida)
    {
        /* this serveix per referir-se a la variable de la classe, en aquest cas velocitat. 
        És una manera de diferenciar-la de la variable que li passem com a paràmetre, encara que tinguin el mateix nom. */

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
        /* void vol dir que la funció no retorna res. Si volguessis que retornés un enter, per exemple, hauries d'escriure int enlloc de void.
        Si volguessis que retornes una llista de strings, hauries d'escriure List<string> enlloc de void. Més endavant veuràs més tipus de dades */
        // Aquí pots fer coses que només vols executar una vegada, com per exemple assignar un color aleatori al cotxe, o engegar-lo:

        motorEnMarxa = prepararCotxe(); // cridem a la funció prepararCotxe, que retorna un booleà (True/False, 1/0) que assignem a la variable 

    }

    protected bool prepararCotxe() // El tipus de la funció és bool, perquè retorna un booleà
    {
        // Aquí pots fer coses que només vols executar una vegada
        return true; // retorna true
    }
    

    // Update es crida cada frame, és a dir, 60 vegades per segon
    protected void Update()
    {
        // Aquí pots fer coses que vols que es repeteixin cada frame, com per exemple moure el cotxe, o fer que giri, o que acceleri
    }

    /* Hi ha altres funcions que venen de Unity, com Awake, OnEnable, OnDisable, OnGUI, etc. 
    Com t'he explicat, pots fer sub-classes, que hereden les propietats, funcions, etc de la superclasse. 
    Cotxe és una subclasse de MonoBehaviour, que és una classe de Unity com hem vist a dalt, i que conté totes aquestes funcions */


    // A partir d'aquí és una mica més avançat

    // Creem per exemple una nova classe, que només estarà disponible des de ScriptCotxe, roda
    protected class Roda // No volem que heredi de cap classe, per tant no posem : MonoBehaviour
    {
        string tipus; // tipus de roda: hivern, estiu, pluja, muntanya
        float friccio; // fricció de la roda, un decimal
        int durabilitat; // durabilitat de la roda, un enter

        // Creem un constructor per a la classe Roda
        public Roda(string tipus = "estiu") 
        {
            /* Fixat que hem assignat un valor per defecte a tipus, en cas que no li passem cap paràmetre.
            Per tant, per crear una roda, podem cridar aquest constructor de dues maneres diferents per crear una roda d'estiu:
            Roda roda1 = new Roda(); on el tipus serà estiu, o bé
            Roda roda2 = new Roda("estiu"); on el tipus serà l'especificat. */

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

    /* Ara que ja tenim les rodes, anem a posar-li al cotxe. Quan volem guardar un conjunt d'elements, tenim diferents tipus de dades que podem utilitzar.
    Tenim per exemple les llistes, els vectors, les tuples, etc. Una llista i un vector (array) són molt semblants, però els vectors tenen una mida fixa, 
    i són més ràpids. En canvi, les llistes poden créixer i disminuir de mida, però són més lentes.
    Si fem servir una llista, volem que contingui rodes. Per tant, diriem:
    
    List<Roda> rodes[4] = new List<Roda>();
    
    Aquí, especifiquem que la variable "rodes" és una llista amb elements de tipus Roda, i li assignem el següent valor: un nou objecte de tipus llista, que conté rodes.
    Això és una mica liat però ja t'ho explicaré millor.

    Fem-ho però amb vectors:
    */

    protected Roda[] rodes = new Roda[4]; // [] vol dir que és un vector, i [4] que té 4 elements

    // Anem a crear un parell de funcions per posar les rodes al cotxe i comprovar que estiguin bé

    protected void posarRodes(string tipus) // No volem que retorni res, per tant void, que vol dir buit, o "sense res", que és el que retorna: no res
    {
        /* Hem inicialitzat el vector, però no hem creat les rodes. 
        Quan inicialitzes un vector o una llista, igual que quan fem "int x;", estem creant l'estructura on es guardaran les dades,
        però no les dades a guardar. Es com construir un moble amb 4 calaixos, però no poses res a dins. 
        Ara, anem a posar les rodes dins dels calaixos, cadascuna de les 4 posicions del vector. */

        // Els bucles són una altra eina molt útil. Ens permeten repetir una acció un nombre determinat de vegades.
        // En aquest cas, volem repetir 4 vegades la mateixa acció, que és crear una roda i posar-la al vector.
        /* Pots llegir el bucle com: creem una variable "i" que val 0. Mentre "i" sigui més petit que 4, fem el que hi ha dins del bucle, i sumem 1 a "i".
        Per tant, per a cada i, que en aquest cas és 0, 1, 2 i 3, creem una roda i la posem al vector.
        S'acostuma a dir que la i és 0, ja que les llistes etc comencen a comptar des de 0, no des de 1, 
        i per això diem mentres i sigui més petit que 4, i no més petit o igual que 4. */

        for (int i = 0; i < rodes.Length; i++) 
        {
            /* rodes.Length és la mida del vector, en aquest cas 4. Quan tens una classe amb paràmetres o funcions a dins,
            pots accedir a aquests paràmetres o funcions amb el punt. Per exemple, si tens una classe "Persona", amb un paràmetre "edat",
            pots accedir a l'edat de la persona amb persona.edat.*/
            rodes[i] = new Roda(tipus); // Assignem a la posició i del vector rodes una nova roda del tipus indicat
        }
    }

    // Ara, comprovem que les rodes s'hagin creat bé, mostrant la informació al terminal:
    protected void comprovarRodes()
    {
        foreach (Roda roda in rodes) 
        {
            /* Foreach és un altre tipus de bucle. Enlloc de fer servir un index, com el "i" del for,
            fa servir directament l'objecte que hi ha dins de la llista o vector. Són més fàcils de llegir:
            Per a cada roda en rodes, fes el que hi ha dins del bucle.
            És important saber que no es pot modificar propietats de l'objecte roda, però sí que es pot llegir.
            Si es volen modificar, s'ha de fer servir un for normal. */
            Debug.Log("Roda: " + roda.tipus + ", fricció: " + roda.friccio + ", durabilitat: " + roda.durabilitat);
            // Debug.Log() és una funció que imprimeix al terminal el que hi ha dins dels parèntesis, com print però per Unity.
            /* Fixat que tipus, fricció i durabilitat són propietats de la roda, i per tant s'accedeix amb el punt, però surten subratllades en vermell.
            Això és perquè són privades, i no es poden modificar des de fora de la classe. Per això, hem de canviar-les a públiques.
            Ves a la classe Roda i posa "public" davant de les propietats. public string tipus, etc. */
        }
    }
    
    /* Fixat també que a sobre de posarRodes i comprovarRodes diu que hi ha 0 referències. Això és perquè no estem cridant aquestes funcions enlloc.
    Torna a la funció prepararCotxe(), i crida a les dues funcions, amb el tipus de roda que vulguis o sense cap. posarRodes(); etc. */
}


// Per acabar, creem una subclasse de cotxe, CotxeMuntanya, que hereta de Cotxe.
// Això vol dir que CotxeMuntanya té totes les propietats i funcions de Cotxe, i a més, les seves pròpies.
public class CotxeMuntanya : Cotxe
{
    // Els cotxes de muntanya són més lents, i sempre porten rodes de muntanya.
    // Si volem canviar alguna cosa de la superclasse, podem sobre-escriure els mètodes.
    // El constructor no es pot sobreescriure, però podem fer modificacions addicionals un cop s'ha cridat.
    public CotxeMuntanya(int velocitat, int velocitatMaxima, string marca, string model, string color, int vida) : base(velocitat, velocitatMaxima, marca, model, color, vida) 
    {
        // Primer, s'executa el constructor de la superclasse amb els paràmetres. Després, ho podem modificar:
        this.velocitatMaxima = velocitatMaxima / 2; // Dividim la velocitat màxima per 2, perquè els cotxes de muntanya són més lents.
        this.vida = vida * 2; // Multipliquem la vida per 2, perquè els cotxes de muntanya són més resistents.
    }

    // Amb la resta de mètodes, els podem canviar per complet, amb override (sobreescriure), o afegir coses, amb new (nou).
    new bool prepararCotxe()
    {
        // Els cotxes de muntanya sempre porten rodes de muntanya:
        posarRodes("muntanya");
        return true;
    }

}