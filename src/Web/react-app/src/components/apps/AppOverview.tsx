import { useContext } from "react";
import { AppContext } from "./AppContext";
import { useForm } from "react-hook-form";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";

export default function AppOverview() {
  const app = useContext(AppContext)!;
  const domain = `https://app-${app.id}.app.mycrocloud.com`;
  return (
    <div className="p-2">
      <table>
        <tbody>
          <tr>
            <td>Name</td>
            <td>{app.name}</td>
          </tr>
          <tr>
            <td>Description</td>
            <td>{app.description}</td>
          </tr>
          <tr>
            <td>Created at</td>
            <td>{new Date(app.createdAt).toDateString()}</td>
          </tr>
          <tr>
            <td>Domain</td>
            <td>{domain}</td>
          </tr>
        </tbody>
      </table>
      <hr className="mb-2" />
      <RenameComponent />
      <ChangeModeComponent />
      <DeleteComponent />
    </div>
  );
}

type RenameFormInput = {
  name: string;
};
function RenameComponent() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();

  const { register, handleSubmit } = useForm<RenameFormInput>({
    defaultValues: {
      name: app.name,
    },
  });
  const onSubmit = async (input: RenameFormInput) => {
    try {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/rename`, {
        method: "PATCH",
        headers: {
          "content-type": "application/json",
          Authorization: `Bearer ${accessToken}`,
        },
        body: JSON.stringify(input),
      });
      if (res.ok) {
        toast("Renamed app");
      }
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input type="text" {...register("name")} className="border px-1 py-0.5" />
      <button type="submit" className="ms-2 text-primary">
        Rename
      </button>
    </form>
  );
}

function DeleteComponent() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const handleDeleteClick = async () => {
    if (confirm("Are you sure want to delete this app?")) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        toast("Deleted app");
        navigate("/apps");
      }
    }
  };
  return (
    <div className="mt-2">
      <button
        type="button"
        className="text-red-600"
        onClick={handleDeleteClick}
      >
        Delete
      </button>
    </div>
  );
}
function ChangeModeComponent() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const handleChangeModeClick = async () => {
    if (confirm("Are you sure want to change mode?")) {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/mode`, {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      if (res.ok) {
        toast("Changed mode");
        navigate("/apps");
      }
    }
  };
  return (
    <div className="mt-2">
      <div>Mode: {app.status}</div>
      <button
        type="button"
        className="text-secondary"
        onClick={handleChangeModeClick}
      >
        Change
      </button>
    </div>
  );
}
