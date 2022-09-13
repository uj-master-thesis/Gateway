import pytest
import http.client
import json
import requests
import time
import uuid

base_url = "http://localhost:5000"
guid = uuid.uuid4()
thread = {
    "name": f"Example_Thread{guid}",
    "description": "This is example thread"
}
post = {
    "postName": f"Example_Post{guid}",
    "description": "This is example post",
    "threadName": f"Example_Thread{guid}",
    "username": "testUser@domain.com"
}

comment = {
    "text": "this is content of comment",
    "username": "testUser@domain.com",
    "postName": f"Example_Post{guid}",
}

vote = {
    "voteType": 0,
    "username": "testUser@domain.com",
    "postName": f"Example_Post{guid}",
}
payload = "{\"client_id\":\"EPHxSVWFHoRH3nYhMZVPavYW7C3Q5sdN\",\"client_secret\":\"OiTd2qGA1y_czj3aeEakT0rtl_ohhqpm3y8KtzaHoatPR7mkDwuXADX-lCzgTz6q\",\"audience\":\"https://micronews/api\",\"grant_type\":\"client_credentials\"}"


def test_end_to_end():
    assert make_post_request(thread, "thread").ok
    assert make_post_request(post, "post").ok
    assert make_post_request(comment, "comment").ok
    assert make_post_request(vote, "vote").ok
    list_of_threads = requests.get(f"{base_url}/api/thread").json()
    assert any(thread["name"] == existing_thread["name"] for existing_thread in list_of_threads)
    list_of_posts = requests.get(f"{base_url}/api/post").json()
    print(list_of_posts)
    assert any(post["postName"] == existing_post["postName"] for existing_post in list_of_posts)
    list_of_comments = requests.get(f"{base_url}/api/comment/by-post/{post['postName']}").json()
    assert any(comment["text"] == existing_comment["text"] for existing_comment in list_of_comments)

def make_post_request(json_object, path_to_add):
    headers = {"Authorization": f"Bearer {get_barear_token()}"}
    return requests.post(f"{base_url}/uploader/{path_to_add}", json=json_object, headers=headers)

def get_barear_token():
    conn = http.client.HTTPSConnection("dev-x5ozep80.eu.auth0.com")
    headers = {'content-type': "application/json"}
    conn.request("POST", "/oauth/token", payload, headers)
    res = conn.getresponse()
    data = res.read()
    return json.loads(data.decode("utf-8"))["access_token"]
