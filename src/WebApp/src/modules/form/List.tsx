import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import IForm from "./IForm";
export default function List() {
  const { getAccessTokenSilently } = useAuth0();

  const [forms, setForms] = useState<IForm[]>([]);

  useEffect(() => {
    const getForms = async () => {
      const accessToken = await getAccessTokenSilently();
      const response = await fetch("/formapi/forms", {
        headers: { Authorization: `Bearer ${accessToken}` },
      });
      const data = await response.json();
      setForms(data);
    };
    getForms();
  }, []);
  return (
    <div>
      <h1 className="font-bold">Your Forms</h1>
      <Link
        to="new"
        className="mt-3 inline-block bg-primary px-3 py-1 text-white"
      >
        New
      </Link>
      <div className="mt-3">
        <ul>
          {forms.map((form) => (
            <li key={form.id}>
              <Link to={`${form.id}`}>
                <h2>{form.name}</h2>
              </Link>
              <p className="text-sm text-slate-700">{form.description}</p>
              <p className="text-sm text-slate-700">
                {new Date(form.createdAt).toDateString()}
              </p>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
