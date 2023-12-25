import { useContext } from "react";
import { AppContext } from "./AppContext";
import { useForm } from "react-hook-form";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";

export default function AppOverview() {
  const app = useContext(AppContext)!;
  const domain = `https://app-${app.id}.app.mycrocloud.com`;
  return (
    <>
      <div className="border">
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
      </div>
      <RenameForm />
    </>
  );
}

type RenameFormInput = {
  name: string;
};
function RenameForm() {
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
      <div>
        <input type="text" {...register("name")} />
        <button type="submit">Rename</button>
      </div>
    </form>
  );
}
