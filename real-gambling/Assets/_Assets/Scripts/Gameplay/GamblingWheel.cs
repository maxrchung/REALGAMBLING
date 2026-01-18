using System.Linq;
using GambleCore.Gambling;
using GambleCore.Interface;
using GambleCore;
using GambleCore.Util;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamblingWheelController : AbstractWheel
{
    public override int Height => 3;
    private readonly ISymbol[] _symbols;
    protected override ISymbol[] Symbols => _symbols.ToArray();

    public GamblingWheelController(int faceCount = 5)
    {
        _symbols = new ISymbol[faceCount];
    }

    public void RandomizeSymbols(IRng rng)
    {
        for (var i = 0; i < _symbols.Length; ++i)
            _symbols[i] = ReelIconAdapter.Random(rng);
    }

    public void PrintSymbols()
    {
        Debug.Log(string.Join(" ", _symbols.Select(symbol => symbol.ToString())));
    }
}

public class GamblingWheel : MonoBehaviour
{
    private GamblingController _gamblingController = new GamblingController();
    private IGamblingBoard _board;
    private RoomController _roomController;
    private GamblingWheelController _wheelController;

    public int FaceCount = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _board = _gamblingController.CreateBoard(0);
        _roomController = FindObjectsByType<RoomController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .First();
        _wheelController = new GamblingWheelController();
        RandomizeSymbols();
        _board.AddWheel(_wheelController);
    }

    void RandomizeSymbols()
    {
        var rng = DeterministicRng.CreateStream(0, "GamblingWheel");
        _wheelController.RandomizeSymbols(rng);
        _wheelController.PrintSymbols();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug spinning wheel on key press
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Space key was pressed!");
            var steps = _board.GetRandomSteps();
            _board.PerformSteps(steps);
            Debug.Log(string.Join(" ", _wheelController.ShownSymbols.Select(symbol => symbol.ToString())));
        }
    }


}