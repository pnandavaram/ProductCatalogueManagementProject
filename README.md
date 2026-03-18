# Product Catalogue Management

## 📌 Overview
This is a .NET WEB API application for managing products.

## 🛠️ Technologies
- .NET WEB API
- Entity Framework Core
- SQLite
- NUnit
- Moq

## 📐 Architecture Diagram

Client → API/Controller Layer → Service Layer → Repository Layer → Database  
                                    ↓  
                                Inventory API

## 🚀 Features
- Add new product
- Get all products
- Get product by ID
- Inventory integration

## 🚀 Further Improvements
- Implement update and delete product endpoints
- Implement Authentication and Authorization


## 🔌 API Endpoints

    ### Get All Products
    GET /api/products
    
    ### Get Product By Id
    GET /api/products/{id}
    
    ### Add Product
    POST /api/products

## 📦 Sample Request

    POST /api/products

    {
      "name": "Laptop",
      "description": "Software laptop"
    }

    ## 📦 Sample Response

    {
      "id": "e01648dd-7c1f-4ae7-9b65-582474394e02",
      "name": "Laptop",
      "description": "Software laptop",
      "price": 1200,
      "stock": 5,
      "status": "OK"
    }

    GET /api/products

    ## 📦 Sample Response
    [
      {
        "id": "e01648dd-7c1f-4ae7-9b65-582474394e02",
        "name": "Laptop",
        "description": "Software laptop",
        "price": 1200,
        "stock": 5,
        "status": "OK"
      },
      {
        "id": "a12345dd-7c1f-4ae7-9b65-582474394e02",
        "name": "Phone",
        "description": "Smartphone",
        "price": 800,
        "stock": 10,
        "status": "OK"
      },
      {		
        "id": "b12345dd-7c1f-4ae7-9b65-582474394e02",
        "name": "Books",
        "description": "Fictional Books",
        "price": null,
        "stock": null,
        "status": "Inventory Data Unavailable"
      }
    ]

    GET /api/products/{id}

    ## 📦 Sample Response

    {
      "id": "e01648dd-7c1f-4ae7-9b65-582474394e02",
      "name": "Laptop",
      "description": "Software laptop",
      "price": 1200,
      "stock": 5,
      "status": "OK"
    }


## 🧪 Testing
- NUnit for unit tests
- SQLite (In-Memory) for repository tests
- Moq for mocking dependencies




