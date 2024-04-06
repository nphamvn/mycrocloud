import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import IForm from "./IForm";

interface Submission {
  id: number;
  formId: number;
  createdAt: string;
  values: ISubmissionValue[];
}
interface ISubmissionValue {
  fieldId: string;
  value: any;
}
export default function SubmissionList() {
  const { getAccessTokenSilently } = useAuth0();
  const id = parseInt(useParams()["id"]!);
  const [form, setForm] = useState<IForm>();
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  useEffect(() => {
    const getForm = async () => {
      const accessToken = await getAccessTokenSilently();
      const response = await fetch(`/formapi/forms/${id}`, {
        headers: { Authorization: `Bearer ${accessToken}` },
      });
      const data = (await response.json()) as IForm;
      setForm(data);
    };
    const getSubmissions = async () => {
      const accessToken = await getAccessTokenSilently();
      const response = await fetch(`/formapi/forms/${id}/submissions`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const data = await response.json();
      setSubmissions(data);
    };

    getForm();
    getSubmissions();
  }, []);

  if (!form) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1>Submission List</h1>
      <table className="w-full">
        <thead>
          <tr>
            <th className="w-64 text-start">Time</th>
            {form.fields.map((field) => (
              <th key={field.id} className="text-start">
                {field.name}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {submissions.map((submission) => (
            <tr key={submission.id}>
              <td>{submission.createdAt}</td>
              {form.fields.map((field) => {
                const value = submission.values.find(
                  (value) => value.fieldId === field.id,
                );
                return <td key={field.id}>{value?.value}</td>;
              })}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
