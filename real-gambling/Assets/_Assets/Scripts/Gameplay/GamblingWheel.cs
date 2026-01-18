using System.Linq;
using GambleCore.Gambling;
using GambleCore.Interface;
using GambleCore.Util;
using UnityEngine;

public class GamblingWheelController : AbstractWheel
{
    private readonly ISymbol[] _symbols;
    protected override ISymbol[] Symbols { get; }

    public GamblingWheelController(int faceCount = 5)
    {
        _symbols = new ISymbol[faceCount];
    }

    public void RandomizeSymbols(IRng rng)
    {
        for (var i = 0; i < _symbols.Length; ++i)
            _symbols[i] = ReelIconAdapter.Random(rng);
    }
}

public class GamblingWheel : MonoBehaviour
{
    private RoomController _roomController;
    private GamblingWheelController _wheelController;

    public int FaceCount = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roomController = FindObjectsByType<RoomController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .First();
        _wheelController = new GamblingWheelController();
        RandomizeSymbols();
    }

    void RandomizeSymbols()
    {
        var rng = DeterministicRng.CreateStream(0, "GamblingWheel");
        _wheelController.RandomizeSymbols(rng);
        Debug.Log("Randomized symbols:");
    }

    // Update is called once per frame
    void Update()
    {
    }
}