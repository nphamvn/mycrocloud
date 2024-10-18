import { PlusIcon } from "@heroicons/react/24/solid";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

interface IColumn {
  id: string;
  name: string;
  type: string;
  details?: any;
}

const mockColumns = [
  {
    id: "1",
    name: "name",
    type: "string",
  },
  {
    id: "2",
    name: "age",
    type: "number",
  },
];

interface IRow {
  [key: string]: any;
}

const mockRows = [
  {
    id: "1",
    name: "John",
    age: 25,
  },
  {
    id: "2",
    name: "Jane",
    age: 30,
  },
];

export default function Edit() {
  const id = useParams().id;

  const [columns, setColumns] = useState<IColumn[]>([]);
  const [rows, setRows] = useState<IRow[]>([]);
  const [selectedColumn, setSelectedColumn] = useState<IColumn | null>(null);

  useEffect(() => {
    const fetchColumns = async () => {
      setColumns(mockColumns);
    };

    const fetchRows = async () => {
      setRows(mockRows);
    };

    fetchColumns();
    fetchRows();
  }, []);

  const handleAddColumnClick = () => {
    const newColumn: IColumn = {
      id: String(columns.length + 1),
      name: "",
      type: "string",
    };

    setColumns([...columns, newColumn]);
    setSelectedColumn(newColumn);
  };

  const handleColumnClick = (column: IColumn) => {
    setSelectedColumn(column);
  };

  const handleColumnChange = (key: string, value: any) => {
    if (!selectedColumn) return;

    setSelectedColumn({
      ...selectedColumn,
      [key]: value,
    });
  };

  const handleCellChange = (rowId: string, columnId: string, value: any) => {
    const newRows = rows.map((row) => {
      if (row.id === rowId) {
        return {
          ...row,
          [columnId]: value,
        };
      }

      return row;
    });

    setRows(newRows);
  };

  return (
    <div>
      <div className="mt-2">
        <input
          type="text"
          className="border p-1.5"
          placeholder="Name"
          autoComplete="off"
        />
      </div>
      <div className="mt-2 flex">
        <div>
          <table className="border-collapse border">
            <thead>
              <tr>
                <th></th>
                {columns.map((column) => (
                  <th
                    key={column.id}
                    className="cursor-pointer border"
                    onClick={() => handleColumnClick(column)}
                  >
                    {column.name}
                  </th>
                ))}
                <th className="border px-2">
                  <button onClick={handleAddColumnClick}>
                    <PlusIcon className="h-4 w-4" />
                  </button>
                </th>
              </tr>
            </thead>
            <tbody>
              {rows.map((row) => (
                <tr key={row.id} className="">
                  <td className="border px-1">
                    <input type="checkbox" />
                  </td>
                  {columns.map((column) => (
                    <td key={column.id} className="border">
                      <input
                        type="text"
                        className="p-1.5"
                        value={row[column.name]}
                        onChange={(e) =>
                          handleCellChange(row.id, column.id, e.target.value)
                        }
                      />
                    </td>
                  ))}
                  <td className="border"></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="ms-auto border p-2">
          {selectedColumn && (
            <div>
              <div>
                <input
                  type="text"
                  className="border p-1.5"
                  placeholder="Name"
                  value={selectedColumn.name}
                  onChange={(e) => handleColumnChange("name", e.target.value)}
                />
              </div>
              <div className="mt-2 flex">
                <label className="mr-2">Type:</label>
                <select
                  value={selectedColumn.type}
                  onChange={(e) => handleColumnChange("type", e.target.value)}
                  className="ms-auto"
                >
                  <option value="string">String</option>
                  <option value="number">Number</option>
                  <option value="boolean">Boolean</option>
                  <option value="date">Date</option>
                  <option value="select">Enum</option>
                </select>
              </div>
              {selectedColumn.type === "select" && (
                <div className="mt-2">
                  <label className="block">Options</label>
                  <input
                    type="text"
                    className="border p-1.5"
                    placeholder="Option 1"
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        handleColumnChange("details", {
                          ...selectedColumn.details,
                          options: [
                            ...(selectedColumn.details?.options || []),
                            {
                              id: String(
                                selectedColumn.details?.options?.length,
                              ),
                              text: e.currentTarget.value,
                            },
                          ],
                        });

                        e.currentTarget.value = "";
                      }
                    }}
                  />
                  {selectedColumn.details?.options.map(
                    (option: { id: string; text: string }) => (
                      <div key={option.id} className="mt-1">
                        <input
                          type="text"
                          className="border p-1.5"
                          value={option.text}
                        />
                      </div>
                    ),
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
