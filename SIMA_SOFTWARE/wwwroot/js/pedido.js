let total = 0;
let items = [];

function agregarItem() {
    let select = document.getElementById("productoSelect");
    let producto = select.options[select.selectedIndex].text;
    let precio = parseFloat(select.options[select.selectedIndex].dataset.precio);
    let cantidad = parseInt(document.getElementById("cantidad").value);

    if (!cantidad || cantidad <= 0) {
        alert("Ingrese una cantidad válida");
        return;
    }

    let subtotal = precio * cantidad;
    total += subtotal;

    items.push({
        idProducto: parseInt(select.value),
        cantidad: cantidad,
        precio: precio
    });

    let fila = document.createElement("tr");

    fila.innerHTML = `
        <td>${producto}</td>
        <td>${cantidad}</td>
        <td>$${precio}</td>
        <td>$${subtotal}</td>
        <td>
            <button class="btn btn-danger btn-sm" onclick="eliminarFila(this, ${subtotal})">
                ❌
            </button>
        </td>
    `;

    document.getElementById("detalle").appendChild(fila);
    actualizarTotal();
}

function eliminarFila(btn, subtotal) {
    btn.closest("tr").remove();
    total -= subtotal;
    actualizarTotal();
}

function actualizarTotal() {
    document.getElementById("total").innerText = total.toFixed(2);
}

function guardarPedido() {
    let clienteId = document.getElementById("clienteSelect").value;

    if (items.length === 0) {
        alert("Agregá al menos un producto");
        return;
    }

    let data = {
        idCliente: parseInt(clienteId),
        detalles: items
    };

    fetch('/Pedido/Guardar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(res => res.json())
        .then(res => {
            alert("Venta guardada correctamente");
            location.reload();
        })
        .catch(err => console.error(err));
}