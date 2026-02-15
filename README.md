# AI Development Workshop

Goal of this repository is to collect some examples of ollama usage.

### __Installation__
```bash
winget install --id Ollama.Ollama
```

### __Using CLI__
Run model
```bash
ollama run phi4-mini:latest
```
Get the list of local models
```bash
ollama list
```
Show model info
```bash
ollama show phi4-mini
```
Remove a model
```bash
ollama rm gemma3:4b
```
Pull model from remote registry (default ollama)
```bash
ollama pull gemma3:4b
```
Get the list of models run
```bash
ollama ps
```
Get tokens performance results (useful for cost estimation)
```bash
ollama run phi4-mini --verbose "Can you make a simple example of rest api handler only get method? No explanation required."
```
cURL request
```powershell
$a = curl http://localhost:11434/api/chat -d '{
  "model": "phi4-mini",
  "messages": [{
    "role": "user",
    "content": "Hello"
  }],
  "stream": false
}' | ConvertFrom-Json
```

### __Custom model__
Build the custom model
```bash
ollama create phi4-mini-custom -f Modelfile

ollama run phi4-mini-custom --verbose "Can you make a simple example of rest api handler only get method? No explanation required."
```

### .Net application