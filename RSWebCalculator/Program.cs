using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Dictionary<string, int> baseSignatures = new Dictionary<string, int>()
{
    {"Ice", 4300}, {"Aluminium", 4285}, {"Iron", 4270}, {"Silicon", 4255},
    {"Copper", 4240}, {"Corundum", 4225}, {"Quartz", 4210}, {"Tin", 4195},
    {"Hephaestanite", 4180}, {"Torite", 3900}, {"Agricium", 3885}, {"Tungsten", 3870},
    {"Titanium", 3855}, {"Aslarite", 3840}, {"Laranite", 3825}, {"Bexalite", 3600},
    {"Gold", 3585}, {"Borase", 3570}, {"Taranite", 3555}, {"Beryl", 3540},
    {"Lindinium", 3400}, {"Riccite", 3385}, {"Ouratite", 3370}, {"Savrillium", 3200},
    {"Stileron", 3185}, {"Quantainium", 3170}
};

app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <title>Malte's Wonderful RS Calculator</title>
    <style>
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            background-color: #1e1e24; 
            color: #f5f5f5; 
            display: flex; 
            justify-content: center; 
            margin-top: 100px; 
        }
        .main-wrapper {
            display: flex;
            gap: 30px;
            align-items: flex-start;
        }
        .container { 
            background-color: #2b2b36; 
            padding: 40px; 
            border-radius: 8px; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.5); 
            text-align: center; 
        }
        input { 
            padding: 12px; 
            font-size: 16px; 
            border: none; 
            border-radius: 4px; 
            margin-right: 10px; 
            width: 200px; 
            outline: none; 
        }
        button { 
            padding: 12px 20px; 
            font-size: 16px; 
            border: none; 
            border-radius: 4px; 
            background-color: #0078D7; 
            color: white; 
            cursor: pointer; 
            transition: 0.2s; 
        }
        button:hover { background-color: #005a9e; }
        #result { 
            margin-top: 25px; 
            font-size: 18px; 
            font-weight: bold; 
            color: #4CAF50; 
            height: 25px; 
        }
        .error { color: #ff5252 !important; }
        
        /* Sidebar Styles */
        .sidebar {
            background-color: #2b2b36; 
            padding: 25px 35px; 
            border-radius: 8px; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.5); 
            text-align: left;
            min-width: 250px;
        }
        .sidebar h3 {
            margin-top: 0;
            border-bottom: 1px solid #444;
            padding-bottom: 15px;
            text-align: center;
        }
        .mineral-block {
            margin-bottom: 15px;
        }
        .mineral-name {
            font-weight: bold;
            color: #c7a613ff;
            font-size: 16px;
        }
        .mineral-values {
            font-size: 14px;
            color: #ccc;
            margin-top: 5px;
            font-family: monospace;
        }
        /* Make the bold numbers pop a bit more */
        .mineral-values b {
            color: #ffffffff;
            font-size: 15px;
        }
    </style>
</head>
<body>
    <div class='main-wrapper'>
        <div class='container'>
            <h2>Malte's Wonderful RS Calculator</h2>
            <div>
                <input type='number' id='sigInput' placeholder='RS here...' onkeypress='handleEnter(event)' />
                <button onclick='calculate()'>Search..</button>
            </div>
            <div id='result'></div>
        </div>

        <div class='sidebar'>
            <h3>Cheat sheet for the best ores</h3>
            
            <div class='mineral-block'>
                <div class='mineral-name'>Quantainium</div>
                <div class='mineral-values'><b>1: 3170</b> | <b>2: 6340</b> | <b>3: 9510</b></div>
            </div>
            
            <div class='mineral-block'>
                <div class='mineral-name'>Ouratite</div>
                <div class='mineral-values'><b>1: 3370</b> | <b>2: 6740</b> | <b>3: 10110</b></div>
            </div>
            
            <div class='mineral-block'>
                <div class='mineral-name'>Borase</div>
                <div class='mineral-values'><b>1: 3570</b> | <b>2: 7140</b> | <b>3: 10710</b></div>
            </div>
            
            <div class='mineral-block'>
                <div class='mineral-name'>Gold</div>
                <div class='mineral-values'><b>1: 3585</b> | <b>2: 7170</b> | <b>3: 10755</b></div>
            </div>
        </div>
    </div>

    <script>
        function handleEnter(e) {
            if(e.key === 'Enter') calculate();
        }

        async function calculate() {
            const val = document.getElementById('sigInput').value;
            const resultDiv = document.getElementById('result');
            
            if(!val) {
                resultDiv.innerText = 'Please enter a value.';
                resultDiv.className = 'error';
                return;
            }

            resultDiv.innerText = 'Calculating...';
            resultDiv.className = '';

            const response = await fetch('/api/calc?val=' + val);
            const text = await response.text();
            
            if(text.includes('No match')) {
                resultDiv.className = 'error';
            }
            resultDiv.innerText = text;
        }
    </script>
</body>
</html>
", "text/html"));

// 2. The API Endpoint that does the math
app.MapGet("/api/calc", (int? val) =>
{
    if (val == null) return "> Invalid input.";
    int targetValue = val.Value;
    
    foreach (var signature in baseSignatures)
    {
        if (targetValue % signature.Value == 0)
        {
            int count = targetValue / signature.Value;
            if (count >= 1 && count <= 10)
            {
                return $"({signature.Key.ToUpper()}, count: {count})";
            }
        }
    }
    return "Oh no! Ore not found. Misstyped?";
});

app.Run();