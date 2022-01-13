curl --location --request PUT 'http://127.0.0.1:4984/feedlotdb/' \  
--header 'Content-Type: application/json' \
--data-raw '{
    "bucket": "feedlot", 
    "num_index_replicas": 0 
    }