import { useEffect, useState } from "react";
import Header from "./header";
import { useAuth0 } from "@auth0/auth0-react";

export default function PeopleList() {
  const { getAccessTokenSilently } = useAuth0();
  const [people, setPeople] = useState<IUser[]>([]);

  useEffect(() => {
    const fetchPeople = async () => {
      const accessToken = await getAccessTokenSilently();
      const people = await fetch(
        `${import.meta.env.VITE_BASE_CHAT_API_URL}/people`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      ).then((res) => res.json() as Promise<IUser[]>);
      setPeople(people);
    };

    fetchPeople();
  }, []);

  const addContact = async (id: string) => {
    const accessToken = await getAccessTokenSilently();
    const response = await fetch(
      `${import.meta.env.VITE_BASE_CHAT_API_URL}/contacts`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ id }),
      },
    );

    if (response.ok) {
      setPeople((people) =>
        people.map((person) =>
          person.id === id ? { ...person, isContact: true } : person,
        ),
      );
    }
  };

  return (
    <div>
      <Header activeTab="people" />
      <ul className="mt-3 max-w-md divide-y divide-gray-200">
        {people.map((person) => (
          <li key={person.id} className="p-3">
            <div className="flex items-center">
              <div className="me-2 flex-shrink-0">
                <img
                  className="h-8 w-8 rounded-full"
                  src={person.picture}
                  alt={person.fullName}
                />
              </div>
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-medium text-gray-900 ">
                  {person.fullName}
                </p>
              </div>
              <div>
                <button
                  onClick={() => addContact(person.id)}
                  className={person.isContact ? "hidden" : "text-primary"}
                >
                  Add
                </button>
              </div>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
