import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { useAuth0 } from "@auth0/auth0-react";
import { useForm } from "react-hook-form";
import { v4 as uuid } from "uuid";
import { Link, useParams } from "react-router-dom";
import IConversation from "./IConversation";
import { useReferredState } from "../../hooks/useReferredState";

interface IMessage {
  id: number;
  text: string;
  clientId?: string;
  createdAt: string;
  status: "sending" | "sent" | "delivered" | "read" | "failed";
  mine: boolean;
}

type MessageInput = {
  message: string;
};

export default function Chat() {
  const { getAccessTokenSilently } = useAuth0();
  const conversationId = useParams()["conversationId"]; // Get conversationId from URL for real existing conversation (when user clicks on a conversation)
  console.log("conversationId", conversationId);
  const userId = useParams()["userId"]; // Get userId from URL for new conversation (when user clicks on a contact to start a new conversation)
  console.log("userId", userId);

  const conversationIdRef = useRef();

  const conversation = useRef<IConversation>({
    id: conversationId ? parseInt(conversationId) : 0,
    lastMessage: "",
    members: [],
  });

  const url: string = import.meta.env.VITE_BASE_CHAT_API_URL;

  const [messages, messagesRef, setMessages] = useReferredState<IMessage[]>([]);
  const sortedMessages = messages.sort((a, b) => {
    if (a.id > b.id) return -1;
    if (a.id < b.id) return 1;
    return 0;
  });

  const connection = useRef<signalR.HubConnection>();
  useEffect(() => {
    const connect = async () => {
      const accessToken = await getAccessTokenSilently();
      connection.current = new signalR.HubConnectionBuilder()
        .withUrl(`${url}/chat`, {
          accessTokenFactory: () => accessToken,
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect()
        .build();

      connection.current.on("ReceiveChatMessage", handleReceiveMessage);

      await connection.current.start().then(() => console.log("Connected"));
    };

    const fetchMessages = async () => {
      let messages: IMessage[] = [];
      const accessToken = await getAccessTokenSilently();
      if (conversationId) {
        console.log("Fetching messages for conversation", conversationId);
        messages = await fetch(
          `${url}/conversations/${conversationId}/messages`,
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          },
        ).then((res) => res.json() as Promise<IMessage[]>);
      } else if (userId) {
        console.log("Fetching private conversation with user", userId);
        const response = await fetch(`${url}/conversations/private/${userId}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        });
        if (response.ok) {
          const conversation = await response.json();
          conversationIdRef.current = conversation.id;
          messages = await fetch(
            `${url}/conversations/${conversation.id}/messages`,
            {
              headers: {
                Authorization: `Bearer ${accessToken}`,
              },
            },
          ).then((res) => res.json() as Promise<IMessage[]>);
        }
      }
      setMessages(messages.map((msg) => ({ ...msg, status: "sent" })));
    };

    const handleReceiveMessage = (_: IConversation, message: IMessage) => {
      console.log("Received message", message);
      const msg = messagesRef.current.find(
        (m) => m.clientId === message.clientId,
      );
      let newMessages: IMessage[];
      if (msg) {
        newMessages = messagesRef.current.map((msg) => {
          if (msg.clientId === message.clientId) {
            return {
              ...msg,
              id: message.id,
              createdAt: message.createdAt,
              status: "sent",
            };
          }
          return msg;
        });
      } else {
        newMessages = [...messagesRef.current, message];
      }
      setMessages(newMessages);
    };

    connect();
    fetchMessages();
    return () => {
      console.log("Disconnecting...");
      connection.current?.off("ReceiveChatMessage", handleReceiveMessage);
      connection.current
        ?.stop()
        .then(() => console.log("Disconnected"))
        .catch((err) => console.error(err));
    };
  }, []);

  const { handleSubmit, register, reset } = useForm<MessageInput>();

  const onSubmit = (messageInput: MessageInput) => {
    const newMessage: IMessage = {
      id: Math.max(...messagesRef.current.map((msg) => msg.id)) + 1,
      text: messageInput.message,
      createdAt: new Date().toString(),
      clientId: uuid(),
      status: "sending",
      mine: true,
    };
    const newMessages = [...messagesRef.current, newMessage];
    setMessages(newMessages);
    connection.current
      ?.invoke(
        "SendChatMessage",
        conversation.current?.id || null,
        conversation.current?.members.map((m) => m.id).join(",") || null,
        messageInput.message,
        newMessage.clientId,
      )
      .then(() => {
        console.log("Message sent");
        reset();
      })
      .catch((err) => {
        console.error(err);
      });
  };
  const timer = useRef<number>();
  const [typing, setTyping] = useState(false);
  const handleMessageInput = () => {
    console.log("Typing...");
    if (timer.current) {
      console.log("Clearing timer");
      window.clearTimeout(timer.current);
    }
    setTyping(true);
    console.log("Setting timer");
    timer.current = window.setTimeout(() => {
      setTyping(false);
    }, 2000);
  };

  useEffect(() => {
    console.log("Typing changed", typing);
    // Send typing indicator except first time when component is mounted
    connection.current
      ?.invoke("SendTypingIndicator", conversation.current?.id || null, typing)
      .then(() => {
        console.log("Typing indicator sent");
      })
      .catch((err) => {
        console.error(err);
      });
  }, [typing]);

  return (
    <div className="max-w-2xl">
      <Link to="..">Back</Link>
      <section className="container mx-auto flex max-h-96 flex-col-reverse overflow-y-auto p-4">
        {sortedMessages.map((message) => {
          if (message.mine) {
            return (
              <div key={message.id} className="flex items-center justify-start">
                <div className="w-3 overflow-hidden">
                  <div className="h-2 origin-bottom-right rotate-45 transform rounded-sm bg-green-400"></div>
                </div>
                <div className="my-1 rounded bg-green-400 px-3">
                  {message.text}
                </div>
              </div>
            );
          } else {
            return (
              <div key={message.id} className="flex items-center justify-end">
                <div className="my-1 rounded-lg bg-blue-200 px-2 py-1.5">
                  {message.text}
                </div>
                <div className="w-3 overflow-hidden ">
                  <div className="h-4 origin-top-left rotate-45 transform rounded-sm bg-blue-200"></div>
                </div>
              </div>
            );
          }
        })}
      </section>
      <form onSubmit={handleSubmit(onSubmit)} className="absolute bottom-0">
        <div className="flex space-x-1">
          <div
            id="typingIndicator"
            style={{ display: typing ? "block" : "none" }}
          >
            Typing...
          </div>
          <input
            type="text"
            {...register("message", { required: true })}
            onInput={handleMessageInput}
            className="w-full border"
          />
          <button type="submit" className="ms-auto">
            Send
          </button>
        </div>
      </form>
    </div>
  );
}
