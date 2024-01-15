import { useContext, useEffect } from "react";
import { AppContext } from "../apps/AppContext";
import { IScheme } from "./IScheme";
import { useAuth0 } from "@auth0/auth0-react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";

type Inputs = {
  name: string;
  type: string;
  openIdConnectIssuer?: string;
  openIdConnectAudience?: string;
};

export default function CreateUpdateScheme() {
  const schemeId = useParams()["schemeId"];
  const editMode = schemeId !== undefined;
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    setError,
  } = useForm<Inputs>({
    defaultValues: {
      name: "",
      type: "openidconnect",
      openIdConnectIssuer: "",
      openIdConnectAudience: "",
    },
  });

  const onSubmit = async (data: Inputs) => {
    if (data.type === "OpenIdConnect") {
      try {
        const res = await fetch(
          `${data.openIdConnectIssuer}.well-known/openid-configuration`,
        );
        if (!res.ok) {
          throw new Error("Invalid issuer");
        }
      } catch (error) {
        setError("openIdConnectIssuer", { message: "Invalid issuer" });
        return;
      }
    }

    const submitData = {
      ...data,
      openIdConnectAuthority: data.openIdConnectIssuer,
    };
    delete submitData.openIdConnectIssuer;
    const accessToken = await getAccessTokenSilently();
    await fetch(
      !editMode
        ? `/api/apps/${app.id}/authentications/schemes`
        : `/api/apps/${app.id}/authentications/schemes/${parseInt(schemeId)}`,
      {
        method: !editMode ? "POST" : "PUT",
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "content-type": "application/json",
        },
        body: JSON.stringify(submitData),
      },
    );
    navigate("../schemes");
  };

  useEffect(() => {
    const getScheme = async () => {
      const accessToken = await getAccessTokenSilently();
      const scheme = (await (
        await fetch(`/api/apps/${app.id}/authentications/schemes/${schemeId}`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as IScheme;
      setValue("name", scheme.name);
      setValue("type", scheme.type);
      setValue("openIdConnectIssuer", scheme.openIdConnectAuthority);
      setValue("openIdConnectAudience", scheme.openIdConnectAudience);
    };
    if (editMode) {
      getScheme();
    }
  }, []);
  return (
    <form className="p-2" onSubmit={handleSubmit(onSubmit)}>
      <h1>Create Scheme</h1>
      <div className="mt-3">
        <label htmlFor="name" className="">
          Name
        </label>
        <input
          {...register("name", { required: true })}
          type="text"
          id="name"
          autoComplete="none"
          className="w-full border border-gray-200 px-2 py-1"
        />
        {errors.name && (
          <p className="text-xs text-red-500">{errors.name.message}</p>
        )}
      </div>
      <div className="mt-2">
        <label htmlFor="type">Type</label>
        <select id="type" {...register("type", { required: true })}>
          <option value="OpenIdConnect">OpenID Connect</option>
        </select>
        {errors.type && (
          <p className="text-xs text-red-500">{errors.type.message}</p>
        )}
      </div>
      <section>
        <div className="mt-2">
          <label htmlFor="issuer">Issuer</label>
          <input
            {...register("openIdConnectIssuer")}
            type="text"
            id="issuer"
            autoComplete="none"
            className="w-full border border-gray-200 px-2 py-1"
          />
          {errors.openIdConnectIssuer && (
            <p className="text-xs text-red-500">
              {errors.openIdConnectIssuer.message}
            </p>
          )}
        </div>
        <div className="mt-2">
          <label htmlFor="audience">Audience</label>
          <input
            {...register("openIdConnectAudience")}
            type="text"
            id="audience"
            autoComplete="none"
            className="w-full border border-gray-200 px-2 py-1"
          />
          {errors.openIdConnectAudience && (
            <p className="text-xs text-red-500">
              {errors.openIdConnectAudience.message}
            </p>
          )}
        </div>
      </section>
      <div className="mt-2 flex">
        <Link
          to="../schemes"
          className="me-2 ms-auto inline-flex items-center bg-secondary px-3 text-white"
        >
          Cancel
        </Link>
        <button
          type="submit"
          className="border border-transparent bg-primary px-3 py-2 text-center font-medium text-white focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-cyan-800"
        >
          Save
        </button>
      </div>
    </form>
  );
}
