let items = [];
let total = 0;

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

    let subtotal = precio * cantidad;
    total += subtotal;

    // 👉 guardamos también el índice para poder eliminar bien
    let item = {
        idProducto: idProducto,
        cantidad: cantidad,
        precio: precio,
        subtotal: subtotal
    };

    items.push(item);

    let index = items.length - 1;

    let fila = document.createElement("tr");

    fila.innerHTML = `
        <td>${producto}</td>
        <td>${cantidad}</td>
        <td>$${precio}</td>
        <td>$${subtotal}</td>
        <td>
            <button class="btn btn-danger btn-sm" onclick="eliminarItem(${index}, this)">
                ❌
            </button>
        </td>
    `;

    document.getElementById("detalle").appendChild(fila);
    actualizarTotal();

    // limpiar input
    document.getElementById("cantidad").value = "";

   
}

function eliminarItem(index, btn) {
    // restar total
    total -= items[index].subtotal;

    // eliminar del array
    items.splice(index, 1);

    // eliminar fila visual
    btn.closest("tr").remove();

    actualizarTotal();
}

function actualizarTotal() {
    document.getElementById("total").innerText = total.toFixed(2);
}

function limpiarPedido() {

    items = [];

    total = 0;

    document.getElementById("detalle").innerHTML = "";

    actualizarTotal();

    document.getElementById("clienteSelect").value = "";

    document.getElementById("cantidad").value = "";
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
        .then(async res => {

            const texto = await res.text();

            console.log("RESPUESTA:");
            console.log(texto);

            let data = JSON.parse(texto);

            // ❌ ERROR CONTROLADO
            if (!data.ok) {

                alert("❌ " + data.mensaje);
                limpiarPedido();

                return;
            }

            // ✅ OK
            alert("✅ Venta guardada correctamente");

            window.location.href = "/Pedido/Index";
        })
        
}

    function cancelarPedido(id) {

        if (!confirm("¿Seguro que querés cancelar el pedido?")) return;

        fetch('/Pedido/Cancelar?id=' + id, {
            method: 'POST'
        })
            .then(() => {
                alert("Pedido cancelado");
                location.reload();
            })
            .catch(err => {
                console.error(err);
                alert("Error al cancelar");
            });
    }

