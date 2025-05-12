// API client for Deck Studio backend

const API_BASE = import.meta.env.VITE_API_BASE || 'http://127.0.0.1:5270';

export interface AIResponse {
  response: string;
  jobSpec?: object | null;
  id: string;
}

const client = {
  conversationId: null as string | null,

  async sendMessageToAI(message: string): Promise<AIResponse> {
    const res = await fetch(`${API_BASE}/ai`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message, id: client.conversationId })
    });
    if (!res.ok) throw new Error('Failed to get AI response');
    const data = await res.json();
    if (data.id) client.conversationId = data.id;
    return data;
  },

  async postJobSpec(jobSpec: object): Promise<string> {
    const res = await fetch(`${API_BASE}/job`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(jobSpec)
    });
    if (!res.ok) throw new Error('Failed to post job spec');
    const data = await res.json();
    return data.status;
  },

  async getLatestJobSpec(): Promise<object | null> {
    const res = await fetch(`${API_BASE}/latest`);
    if (res.status === 404) return null;
    if (!res.ok) throw new Error('Failed to fetch latest job spec');
    return res.json();
  },

  resetConversationId() {
    client.conversationId = null;
  }
};

export default client;
