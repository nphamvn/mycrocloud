import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import IConversation from "./IConversation";
import Header from "./header";

export default function ConversationList() {
  const { getAccessTokenSilently } = useAuth0();
  const [conversations, setConversations] = useState<IConversation[]>([]);

  useEffect(() => {
    const fetchConversations = async () => {
      const accessToken = await getAccessTokenSilently();
      const conversations = await fetch(
        `${import.meta.env.VITE_BASE_CHAT_API_URL}/conversations`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      ).then((res) => res.json() as Promise<IConversation[]>);
      setConversations(conversations);
    };
    fetchConversations();
  }, []);

  return (
    <div>
      <Header activeTab="conversations" />
      <ul className="mt-3 max-w-md divide-y divide-gray-200">
        {conversations.map((conversation) => (
          <li key={conversation.id} className="pb-3 sm:pb-4">
            <Link to={conversation.id.toString()}>
              <div className="flex items-center space-x-4 rtl:space-x-reverse">
                <div className="flex-shrink-0">
                  <img
                    className="h-8 w-8 rounded-full"
                    src={conversation.members[0].picture}
                    alt={conversation.members[0].fullName}
                  />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-medium text-gray-900 dark:text-white">
                    {conversation.members[0].fullName}
                  </p>
                  <p className="truncate text-sm text-gray-500 dark:text-gray-400">
                    {conversation.lastMessage}
                  </p>
                </div>
              </div>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
}
