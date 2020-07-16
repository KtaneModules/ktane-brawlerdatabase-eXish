using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;

public class BrawlerDatabaseScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;
    public Sprite[] characterImgs;
    public Image characterDisp;
    public Text smallDisp;
    public Material[] buttonMats;
    public Renderer[] buttonBorders;
    public Renderer[] buttonInners;
    public GameObject[] errors;
    public GameObject[] buttonTexts;

    private string[] glitches = new string[25];
    private string[] names = new string[] { "Ace", "Airzel", "Alice", "Anubias", "Barodius", "Baron", "Dan", "Fabia", "Gill", "Prince Hydron", "Jake", "Julie", "Kazarina", "Marucho", "Masquerade", "Mira", "Mylene", "Nurzak", "Ren", "Runo", "Sellon", "Shun", "Spectra", "Stoica", "Volt", "King Zenoheld" };
    private string[] homes = new string[] { "Earth", "Gundalia", "N/A", "Neathia", "Vestal" };
    private string[] attributes = new string[] { "Aquos", "Darkus", "Haos", "Pyrus", "Subterra", "Ventus" };
    private string[] factions = new string[] { "Battle Brawlers", "Chaos Army", "Doom Beings", "Twelve Orders", "Vexos" };
    private int[] selectedSmallDisp = new int[4];
    private int[] correctValues = new int[4];
    private Coroutine[] errorCorouts = new Coroutine[4];

    private Coroutine glitch;
    private bool glitchOn = false;
    private int chosenChar = -1;
    private int selectedButton = -1;
    private bool activated = false;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach (KMSelectable obj in buttons)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        GetComponent<KMBombModule>().OnActivate += OnActivate;
    }

    void Start () {
        for (int i = 0; i < 25; i++)
        {
            char[] valids = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            string make = "";
            for (int j = 0; j < 8; j++)
            {
                make += valids[UnityEngine.Random.Range(0, valids.Length)];
            }
            glitches[i] = make;
        }
        chosenChar = UnityEngine.Random.Range(0, characterImgs.Length);
        characterDisp.sprite = characterImgs[chosenChar];
        Debug.LogFormat("[Brawler Database #{0}] The brawler displayed is {1}", moduleId, names[chosenChar]);
        switch (chosenChar)
        {
            case 0: setCorrects(new int[] { chosenChar, 4, 1, 0 }); break;
            case 1: setCorrects(new int[] { chosenChar, 1, 5, 3 }); break;
            case 2: setCorrects(new int[] { chosenChar, 0, 1, 0 }); break;
            case 3: setCorrects(new int[] { chosenChar, 2, 1, 1 }); break;
            case 4: setCorrects(new int[] { chosenChar, 1, 1, 3 }); break;
            case 5: setCorrects(new int[] { chosenChar, 4, 2, 0 }); break;
            case 6: setCorrects(new int[] { chosenChar, 0, 3, 0 }); break;
            case 7: setCorrects(new int[] { chosenChar, 3, 2, 0 }); break;
            case 8: setCorrects(new int[] { chosenChar, 1, 3, 3 }); break;
            case 9: setCorrects(new int[] { chosenChar, 4, 4, 0 }); break;
            case 10: setCorrects(new int[] { chosenChar, 0, 4, 0 }); break;
            case 11: setCorrects(new int[] { chosenChar, 0, 4, 0 }); break;
            case 12: setCorrects(new int[] { chosenChar, 1, 2, 3 }); break;
            case 13: setCorrects(new int[] { chosenChar, 0, 0, 0 }); break;
            case 14: setCorrects(new int[] { chosenChar, 0, 1, 2 }); break;
            case 15: setCorrects(new int[] { chosenChar, 4, 4, 0 }); break;
            case 16: setCorrects(new int[] { chosenChar, 4, 0, 4 }); break;
            case 17: setCorrects(new int[] { chosenChar, 1, 4, 0 }); break;
            case 18: setCorrects(new int[] { chosenChar, 1, 1, 0 }); break;
            case 19: setCorrects(new int[] { chosenChar, 0, 2, 0 }); break;
            case 20: setCorrects(new int[] { chosenChar, 2, 5, 1 }); break;
            case 21: setCorrects(new int[] { chosenChar, 0, 5, 0 }); break;
            case 22: setCorrects(new int[] { chosenChar, 4, 1, 0 }); break;
            case 23: setCorrects(new int[] { chosenChar, 1, 0, 3 }); break;
            case 24: setCorrects(new int[] { chosenChar, 4, 2, 4 }); break;
            case 25: setCorrects(new int[] { chosenChar, 4, 3, 4 }); break;
        }
        Debug.LogFormat("[Brawler Database #{0}] This brawler comes from {1}, where they are apart of the {2}, and they mainly use {3} Bakugan", moduleId, homes[correctValues[1]], factions[correctValues[3]], attributes[correctValues[2]]);
        List<int> error = new List<int>();
        int dec = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < dec; i++)
        {
            int rando = UnityEngine.Random.Range(0, 4); 
            while (error.Contains(rando))
            {
                rando = UnityEngine.Random.Range(0, 4);
            }
            error.Add(rando);
        }
        string[] tempnms = new string[] { "Name", "Home", "Attribute", "Faction" };
        string log = "";
        for (int i = 0; i < error.Count; i++)
        {
            if (i == error.Count - 1 && error.Count != 1)
                log += "and " + tempnms[error[i]];
            else if (i == 0 && error.Count == 1 || error.Count == 2)
            {
                if (error.Count == 1)
                    log += tempnms[error[i]];
                else
                    log += tempnms[error[i]] + " ";
            }
            else
                log += tempnms[error[i]] + ", ";
        }
        Debug.LogFormat("[Brawler Database #{0}] The data values not set properly in the database for this brawler {2} their {1}", moduleId, log, error.Count == 1 ? "is" : "are");
        for (int i = 0; i < error.Count; i++)
        {
            while (correctValues[error[i]] == selectedSmallDisp[error[i]])
            {
                switch (error[i])
                {
                    case 0: selectedSmallDisp[error[i]] = UnityEngine.Random.Range(0, names.Length); break;
                    case 1: selectedSmallDisp[error[i]] = UnityEngine.Random.Range(0, homes.Length); break;
                    case 2: selectedSmallDisp[error[i]] = UnityEngine.Random.Range(0, attributes.Length); break;
                    case 3: selectedSmallDisp[error[i]] = UnityEngine.Random.Range(0, factions.Length); break;
                }
            }
        }
    }

    void OnActivate()
    {
        characterDisp.enabled = true;
        glitch = StartCoroutine(glitchText());
        glitchOn = true;
        for (int i = 0; i < 5; i++)
        {
            buttonTexts[i].SetActive(true);
        }
        for (int i = 0; i < 4; i++)
        {
            if (selectedSmallDisp[i] != correctValues[i])
            {
                errorCorouts[i] = StartCoroutine(errorFlash(i));
            }
        }
        activated = true;
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true && activated)
        {
            if (buttons[0] == pressed && selectedButton != -1)
            {
                pressed.AddInteractionPunch(0.25f);
                audio.PlaySoundAtTransform("dispchange", pressed.transform);
                switch (selectedButton)
                {
                    case 0: selectedSmallDisp[0]++; if (selectedSmallDisp[0] == names.Length) selectedSmallDisp[0] = 0; smallDisp.text = names[selectedSmallDisp[0]]; break;
                    case 1: selectedSmallDisp[1]++; if (selectedSmallDisp[1] == homes.Length) selectedSmallDisp[1] = 0; smallDisp.text = homes[selectedSmallDisp[1]]; break;
                    case 2: selectedSmallDisp[2]++; if (selectedSmallDisp[2] == attributes.Length) selectedSmallDisp[2] = 0; smallDisp.text = attributes[selectedSmallDisp[2]]; break;
                    case 3: selectedSmallDisp[3]++; if (selectedSmallDisp[3] == factions.Length) selectedSmallDisp[3] = 0; smallDisp.text = factions[selectedSmallDisp[3]]; break;
                }
                if (smallDisp.text.Contains(" "))
                {
                    smallDisp.resizeTextMaxSize = 110;
                }
                else
                {
                    smallDisp.resizeTextMaxSize = 150;
                }
            }
            else if (buttons[1] == pressed || buttons[2] == pressed || buttons[3] == pressed || buttons[4] == pressed)
            {
                if (selectedButton == Array.IndexOf(buttons, pressed) - 1)
                    return;
                pressed.AddInteractionPunch(0.5f);
                audio.PlaySoundAtTransform("buttonpress", pressed.transform);
                if (selectedButton != -1)
                {
                    Material[] change2 = new Material[] { buttonMats[1], buttonMats[1], buttonMats[1], buttonMats[1] };
                    buttonBorders[selectedButton].materials = change2;
                    buttonInners[selectedButton].material = buttonMats[0];
                    buttons[selectedButton+1].gameObject.GetComponentInChildren<TextMesh>().color = new Color32(255, 255, 255, 255);
                }
                selectedButton = Array.IndexOf(buttons, pressed) - 1;
                if (glitchOn)
                {
                    StopCoroutine(glitch);
                    glitchOn = false;
                }
                switch (Array.IndexOf(buttons, pressed) - 1)
                {
                    case 0: smallDisp.text = names[selectedSmallDisp[Array.IndexOf(buttons, pressed) - 1]]; break;
                    case 1: smallDisp.text = homes[selectedSmallDisp[Array.IndexOf(buttons, pressed) - 1]]; break;
                    case 2: smallDisp.text = attributes[selectedSmallDisp[Array.IndexOf(buttons, pressed) - 1]]; break;
                    case 3: smallDisp.text = factions[selectedSmallDisp[Array.IndexOf(buttons, pressed) - 1]]; break;
                }
                if (smallDisp.text.Contains(" "))
                {
                    smallDisp.resizeTextMaxSize = 110;
                }
                else
                {
                    smallDisp.resizeTextMaxSize = 150;
                }
                Material[] change = new Material[] { buttonMats[0], buttonMats[0], buttonMats[0], buttonMats[0] };
                buttonBorders[Array.IndexOf(buttons, pressed) - 1].materials = change;
                buttonInners[Array.IndexOf(buttons, pressed) - 1].material = buttonMats[1];
                buttons[Array.IndexOf(buttons, pressed)].gameObject.GetComponentInChildren<TextMesh>().color = new Color32(0, 0, 0, 255);
            }
            else if (buttons[5] == pressed)
            {
                pressed.AddInteractionPunch(0.5f);
                audio.PlaySoundAtTransform("buttonpress", pressed.transform);
                smallDisp.resizeTextMaxSize = 150;
                if (glitchOn)
                {
                    StopCoroutine(glitch);
                    glitchOn = false;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (errorCorouts[i] != null)
                        StopCoroutine(errorCorouts[i]);
                    errors[i].SetActive(false);
                }
                if (selectedButton != -1)
                {
                    Material[] change2 = new Material[] { buttonMats[1], buttonMats[1], buttonMats[1], buttonMats[1] };
                    buttonBorders[selectedButton].materials = change2;
                    buttonInners[selectedButton].material = buttonMats[0];
                    buttons[selectedButton + 1].gameObject.GetComponentInChildren<TextMesh>().color = new Color32(255, 255, 255, 255);
                    selectedButton = -1;
                }
                List<int> incorrect = new List<int>();
                if (correctValues[0] != selectedSmallDisp[0])
                {
                    incorrect.Add(0);
                }
                if (correctValues[1] != selectedSmallDisp[1])
                {
                    incorrect.Add(1);
                }
                if (correctValues[2] != selectedSmallDisp[2])
                {
                    incorrect.Add(2);
                }
                if (correctValues[3] != selectedSmallDisp[3])
                {
                    incorrect.Add(3);
                }
                if (incorrect.Count() != 0)
                {
                    string[] tempnms = new string[] { "Name", "Home", "Attribute", "Faction" };
                    string log = "";
                    for (int i = 0; i < incorrect.Count; i++)
                    {
                        if (i == incorrect.Count - 1 && incorrect.Count != 1)
                            log += "and " + tempnms[incorrect[i]];
                        else if (i == 0 && incorrect.Count == 1 || incorrect.Count == 2)
                        {
                            if (incorrect.Count == 1)
                                log += tempnms[incorrect[i]];
                            else
                                log += tempnms[incorrect[i]] + " ";
                        }
                        else
                            log += tempnms[incorrect[i]] + ", ";
                    }
                    Debug.LogFormat("[Brawler Database #{0}] Manual recovery of data values failed! These data values are now incorrect: The brawler's {1}. Strike!", moduleId, log);
                    glitch = StartCoroutine(glitchText());
                    glitchOn = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (selectedSmallDisp[i] != correctValues[i])
                        {
                            errorCorouts[i] = StartCoroutine(errorFlash(i));
                        }
                    }
                    StartCoroutine(fail());
                    GetComponent<KMBombModule>().HandleStrike();
                }
                else
                {
                    Debug.LogFormat("[Brawler Database #{0}] Manual recovery of data values successful! Module disarmed!", moduleId);
                    moduleSolved = true;
                    audio.PlaySoundAtTransform("success", transform);
                    string[] randoSolves = new string[] { "Nice!", "Cool!", "Good Job!" };
                    int index = UnityEngine.Random.Range(0, randoSolves.Length);
                    smallDisp.text = randoSolves[index];
                    if (index == 2)
                        smallDisp.resizeTextMaxSize = 120;
                    GetComponent<KMBombModule>().HandlePass();
                }
            }
        }
    }

    private void setCorrects(int[] vals)
    {
        for (int i = 0; i < vals.Length; i++)
        {
            correctValues[i] = vals[i];
            selectedSmallDisp[i] = vals[i];
        }
    }

    private IEnumerator fail()
    {
        audio.PlaySoundAtTransform("error", transform);
        yield return new WaitForSecondsRealtime(0.8f);
        audio.PlaySoundAtTransform("failure", transform);
    }

    private IEnumerator glitchText()
    {
        int ct = 0;
        while (true)
        {
            smallDisp.text = glitches[ct];
            ct++;
            if (ct > (glitches.Length-1))
                ct = 0;
            yield return new WaitForSecondsRealtime(0.06f);
        }
    }

    private IEnumerator errorFlash(int num)
    {
        errors[num].SetActive(true);
        yield return new WaitForSeconds(0.1f);
        errors[num].SetActive(false);
        yield return new WaitForSeconds(0.1f);
        errorCorouts[num] = StartCoroutine(errorFlash(num));
    }

    //twitch plays
    private int inDataArray(string str, int n)
    {
        string[] checker = new string[0];
        if (n == 0)
        {
            checker = names;
        }
        else if (n == 1)
        {
            checker = homes;
        }
        else if (n == 2)
        {
            checker = attributes;
        }
        else if (n == 3)
        {
            checker = factions;
        }
        for (int i = 0; i < checker.Length; i++)
        {
            if (checker[i].EqualsIgnoreCase(str))
            {
                return i;
            }
        }
        return -1;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} set <data> <input> [Sets the specified data piece 'data' to 'input'] | !{0} restore/submit [Presses the restore button] | Valid data pieces are name, home, attribute, and faction";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*restore\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[5].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 3)
            {
                string[] valids = new string[] { "name", "home", "attribute", "faction" };
                if (!valids.Contains(parameters[1].ToLower()))
                {
                    yield return "sendtochaterror The specified data type '" + parameters[1] + "' is invalid!";
                    yield break;
                }
                string temp = "";
                for (int i = 2; i < parameters.Length; i++)
                {
                    temp += parameters[i] + " ";
                }
                temp = temp.Trim();
                switch (Array.IndexOf(valids, parameters[1].ToLower()))
                {
                    case 0: if (inDataArray(temp, 0) == -1) { yield return "sendtochaterror The specified input '" + temp + "' is not an option for the name data type!"; yield break; } break;
                    case 1: if (inDataArray(temp, 1) == -1) { yield return "sendtochaterror The specified input '" + temp + "' is not an option for the home data type!"; yield break; } break;
                    case 2: if (inDataArray(temp, 2) == -1) { yield return "sendtochaterror The specified input '" + temp + "' is not an option for the attribute data type!"; yield break; } break;
                    case 3: if (inDataArray(temp, 3) == -1) { yield return "sendtochaterror The specified input '" + temp + "' is not an option for the faction data type!"; yield break; } break;
                }
                if (Array.IndexOf(valids, parameters[1].ToLower()) != selectedButton)
                {
                    buttons[Array.IndexOf(valids, parameters[1].ToLower()) + 1].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                while (selectedSmallDisp[selectedButton] != inDataArray(temp, selectedButton))
                {
                    buttons[0].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (parameters.Length == 3)
            {
                string[] valids = new string[] { "name", "home", "attribute", "faction" };
                if (!valids.Contains(parameters[1].ToLower()))
                {
                    yield return "sendtochaterror The specified data type '" + parameters[1] + "' is invalid!";
                    yield break;
                }
                switch (Array.IndexOf(valids, parameters[1].ToLower()))
                {
                    case 0: if (inDataArray(parameters[2], 0) == -1) { yield return "sendtochaterror The specified input '" + parameters[2] + "' is not an option for the name data type!"; yield break; } break;
                    case 1: if (inDataArray(parameters[2], 1) == -1) { yield return "sendtochaterror The specified input '" + parameters[2] + "' is not an option for the home data type!"; yield break; } break;
                    case 2: if (inDataArray(parameters[2], 2) == -1) { yield return "sendtochaterror The specified input '" + parameters[2] + "' is not an option for the attribute data type!"; yield break; } break;
                    case 3: if (inDataArray(parameters[2], 3) == -1) { yield return "sendtochaterror The specified input '" + parameters[2] + "' is not an option for the faction data type!"; yield break; } break;
                }
                if (Array.IndexOf(valids, parameters[1].ToLower()) != selectedButton)
                {
                    buttons[Array.IndexOf(valids, parameters[1].ToLower()) + 1].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                while (selectedSmallDisp[selectedButton] != inDataArray(parameters[2], selectedButton))
                {
                    buttons[0].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (parameters.Length == 2)
            {
                string[] valids = new string[] { "name", "home", "attribute", "faction" };
                if (valids.Contains(parameters[1].ToLower()))
                    yield return "sendtochaterror The specified data type '"+parameters[1]+"' is valid but please specify something to set this data type to!";
                else
                    yield return "sendtochaterror The specified data type '" + parameters[1] + "' is invalid!";
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a data type and something to set the data type to!";
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = 0; i < 4; i++)
        {
            if (selectedSmallDisp[i] != correctValues[i])
            {
                buttons[i+1].OnInteract();
                yield return new WaitForSeconds(0.1f);
                while (selectedSmallDisp[i] != correctValues[i])
                {
                    buttons[0].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        buttons[5].OnInteract();
    }
}
