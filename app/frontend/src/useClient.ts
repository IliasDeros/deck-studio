import { useState } from 'react'
import client from './client'

export function useClient() {
  const [jobSpec, setJobSpec] = useState<object | null>(null)

  const sendMessage = async (message: string, onBotMessage: (msg: string) => void) => {
    if (!message.trim()) return;
    try {
      const aiReply = await client.sendMessageToAI(message);
      setJobSpec(aiReply.jobSpec ?? null);
      onBotMessage(aiReply.response);
    } catch (err) {
      onBotMessage('Error: Could not reach backend.');
    }
  };

  const confirmJobSpec = async (onBotMessage: (msg: string) => void) => {
    if (!jobSpec) return;
    try {
      await client.postJobSpec(jobSpec);
      onBotMessage('Job Spec confirmed and submitted.');
      setJobSpec(null);
    } catch (err) {
      onBotMessage('Error: Could not confirm job spec.');
    }
  };

  const dismissJobSpec = () => {
    setJobSpec(null);
    client.resetConversationId();
  };

  return { jobSpec, sendMessage, confirmJobSpec, dismissJobSpec };
}
