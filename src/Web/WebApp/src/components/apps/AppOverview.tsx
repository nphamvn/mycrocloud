import { useContext } from "react";
import { AppContext } from "./AppContext";
import { useForm } from "react-hook-form";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import { getAppDomain } from "./service";

export default function AppOverview() {
  const app = useContext(AppContext)!;
  const domain = getAppDomain(app.id);

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
            <td>Updated at</td>
            <td>
              {app.updatedAt ? new Date(app.updatedAt!).toDateString() : "-"}
            </td>
          </tr>
          <tr>
            <td>Domain</td>
            <td className="flex">
              <p className="text-blue-500 hover:underline">{domain}</p>
              <button
                type="button"
                onClick={() => {
                  navigator.clipboard.writeText(domain);
                }}
                className="ms-2 text-xs text-blue-500 hover:underline"
              >
                Copy
              </button>
            </td>
          </tr>
        </tbody>
      </table>
      <hr className="mt-2" />
      <div className="mt-2">
        <RenameComponent />
      </div>
      <hr className="mt-2" />
      <div className="mt-2">
        <ChangeStatusComponent />
      </div>
      <hr className="mt-2" />
      <div className="hidden">
        <h3>Customer Handlers</h3>
        <div>Route Not Found</div>
        <select>
          <option>Default handler</option>
          <option>Redirect to </option>
        </select>
        <select>
          <option>Route 1</option>
          <option>Route 2</option>
        </select>
      </div>
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
function ChangeStatusComponent() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const handleChangeStatusClick = async () => {
    if (
      app.status === "Active" &&
      !confirm("Are you sure want to deactivate the app?")
    ) {
      return;
    }
    const accessToken = await getAccessTokenSilently();
    const status = app.status === "Active" ? "Inactive" : "Active";
    const res = await fetch(`/api/apps/${app.id}/status?status=${status}`, {
      method: "PATCH",
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    if (res.ok) {
      //TODO: update app status in context
      app.status = status;
      toast("Status changed");
      navigate(".");
    }
  };
  function getStatusClass(status: string) {
    switch (status) {
      case "Active":
        return "text-green-500";
      case "Inactive":
        return "text-gray-500";
      case "Blocked":
        return "text-red-500";
      default:
        return "";
    }
  }
  function getChangeStatusButtonClass(status: string) {
    switch (status) {
      case "Active":
        return "text-red-500";
      case "Inactive":
        return "text-green-500";
      case "Blocked":
        return "text-gray-500";
      default:
        return "";
    }
  }
  return (
    <div>
      <div>
        Status: <span className={getStatusClass(app.status)}>{app.status}</span>
      </div>
      <button
        type="button"
        className={`${getChangeStatusButtonClass(app.status)}`}
        disabled={app.status === "Blocked"}
        onClick={handleChangeStatusClick}
      >
        {app.status === "Active" ? "Deactivate" : "Activate"}
      </button>
    </div>
  );
}
