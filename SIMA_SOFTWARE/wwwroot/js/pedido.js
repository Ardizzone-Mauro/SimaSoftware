let items = [];
let total = 0;
const moneyFormatter = new Intl.NumberFormat('es-AR', {
    maximumFractionDigits: 0,
    minimumFractionDigits: 0
});

function agregarItem() {
    let select = document.getElementById("productoSelect");

    let idProducto = parseInt(select.value);
    let producto = select.options[select.selectedIndex].text;
    let precio = parseFloat(select.options[select.selectedIndex].dataset.precio);
    let cantidad = parseInt(document.getElementById("cantidad").value);

    if (!cantidad || cantidad <= 0) {
        alert("Ingrese una cantidad válida");
        return;
    }

    if (!idProducto) {
        alert("Seleccioná un producto");
        return;
    }

    // 🔥 Verificar si el producto ya existe
    let existente = items.find(i => i.idProducto === idProducto);

    if (existente) {
        existente.cantidad += cantidad;
        existente.subtotal = existente.cantidad * existente.precio;
    } else {
        items.push({
            idProducto: idProducto,
            cantidad: cantidad,
            precio: precio,
            subtotal: precio * cantidad
        });
    }

    renderTabla();

    // limpiar input
    document.getElementById("cantidad").value = "";
}

function renderTabla() {
    let tbody = document.getElementById("detalle");
    tbody.innerHTML = "";

    total = 0;

    items.forEach((item, index) => {
        let fila = document.createElement("tr");

        let producto = document.querySelector(`#productoSelect option[value="${item.idProducto}"]`).text;

        total += item.subtotal;

        fila.innerHTML = `
            <td>${producto}</td>
            <td>${item.cantidad}</td>
            <td>$${moneyFormatter.format(item.precio)}</td>
            <td>$${moneyFormatter.format(item.subtotal)}</td>
            <td>
                <button class="btn btn-danger btn-sm" onclick="eliminarItem(${index})">
                    ❌
                </button>
            </td>
        `;

        tbody.appendChild(fila);
    });

    actualizarTotal();
}

function eliminarItem(index) {
    items.splice(index, 1);
    renderTabla();
}

function actualizarTotal() {
    document.getElementById("total").innerText = moneyFormatter.format(total);
}

function guardarPedido() {
    let idCliente = document.getElementById("clienteSelect").value;

    if (!idCliente) {
        alert("Seleccione un cliente");
        return;
    }

    if (items.length === 0) {
        alert("Agregá al menos un producto");
        return;
    }

    fetch('/Pedido/Guardar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            idCliente: parseInt(idCliente),
            detalles: items
        })
    })
        .then(res => {
            if (!res.ok) throw new Error("Error en servidor");
            return res.json();
        })
        .then(data => {
            if (data.ok) {
                alert("✅ Venta guardada correctamente");
                window.location.href = "/Pedido/Index";
            } else {
                alert("❌ Stock insuficiente");
            }
        })
        .catch(error => {
            console.error(error);
            alert("❌ Error en la petición");
        });
}
