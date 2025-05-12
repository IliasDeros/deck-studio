import { useState, useRef, useEffect } from 'react'
import './App.css'
import { Button } from './components/ui/button'
import { Input } from './components/ui/input'
import reactLogo from './assets/react.svg'
import { useClient } from './useClient'

function App() {
  const [messages, setMessages] = useState([
    { sender: 'bot', text: 'Hello! How can I help you today?' }
  ])
  const [input, setInput] = useState('')
  const chatEndRef = useRef<HTMLDivElement | null>(null)
  const { jobSpec, sendMessage, confirmJobSpec, dismissJobSpec } = useClient()

  useEffect(() => {
    chatEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages, jobSpec])

  const handleSend = async (overrideInput?: string) => {
    const messageToSend = overrideInput ?? input;
    if (!messageToSend.trim()) return;
    setMessages((msgs) => [
      ...msgs,
      { sender: 'user', text: messageToSend }
    ])
    setInput('')
    await sendMessage(messageToSend, (botMsg) => {
      setMessages((msgs) => [
        ...msgs,
        { sender: 'bot', text: botMsg }
      ])
    })
  }

  const handleConfirm = async () => {
    await confirmJobSpec((botMsg) => {
      setMessages((msgs) => [
        ...msgs,
        { sender: 'bot', text: botMsg }
      ])
    })
  }

  const handleDismiss = dismissJobSpec

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') handleSend()
  }

  return (
    <div className="flex flex-col w-full bg-gray-100">
      <div className="flex flex-col max-w-xl w-full mx-auto shadow-lg border bg-white h-full">
        <header className="px-6 py-4 border-b bg-white/80 backdrop-blur sticky top-0 z-10 flex items-center gap-3">
          <img src={reactLogo} alt="React logo" className="h-7 w-7" />
          <h1 className="text-xl font-bold tracking-tight text-gray-900">Deck Studio</h1>
        </header>
        <div className="flex-1 overflow-y-auto p-4 space-y-2">
          {messages.map((msg, idx) => (
            <div
              key={idx}
              className={`flex ${msg.sender === 'user' ? 'justify-end' : 'justify-start'}`}
            >
              <div
                className={`rounded-lg px-4 py-2 max-w-xs break-words shadow text-sm ${msg.sender === 'user' ? 'bg-blue-500 text-white' : 'bg-gray-100 text-gray-900 border'}`}
                style={{ textAlign: 'left' }}
              >
                {msg.text}
              </div>
            </div>
          ))}
          {jobSpec && (
            <div className="mt-4 p-3 bg-yellow-50 border border-yellow-300 rounded text-xs text-gray-800 max-w-full flex flex-col items-start">
              <div className="font-semibold mb-1">Job Spec Preview:</div>
              <pre className="whitespace-pre-wrap break-words text-left w-full">{JSON.stringify(jobSpec, null, 2)}</pre>
              <div className="flex gap-2 mt-2">
                <Button onClick={handleConfirm}>Confirm</Button>
                <Button variant="ghost" onClick={handleDismiss}>Dismiss</Button>
              </div>
            </div>
          )}
          <div ref={chatEndRef} />
        </div>
        <form
          className="flex items-center gap-2 p-4 border-t bg-white"
          onSubmit={async e => {
            e.preventDefault();
            await handleSend();
          }}
        >
          <Input
            className="flex-1 rounded border px-3 py-2 focus:outline-none focus:ring focus:border-blue-400 bg-gray-50"
            type="text"
            placeholder="Type your message..."
            value={input}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => setInput(e.target.value)}
            onKeyDown={handleKeyDown}
          />
          <Button type="submit">Send</Button>
        </form>
      </div>
    </div>
  )
}

export default App
