using System;
using System.Linq;
using NUnit.Framework;
using Unity.Collections;

public class ListExtensionsTests
{
    [Test]
    public void RemoveSwapBack_Item()
    {
        var list = new[] { 'a', 'b', 'c', 'd' }.ToList();
        
        Assert.True(list.RemoveSwapBack('b'));
        CollectionAssert.AreEqual(new[] { 'a', 'd', 'c', }, list);
        
        Assert.True(list.RemoveSwapBack('c'));
        CollectionAssert.AreEqual(new[] { 'a', 'd' }, list);
        
        Assert.False(list.RemoveSwapBack('z'));
        CollectionAssert.AreEqual(new[] { 'a', 'd' }, list);
        
        Assert.True(list.RemoveSwapBack('a'));
        CollectionAssert.AreEqual(new[] { 'd' }, list);
        
        Assert.True(list.RemoveSwapBack('d'));
        CollectionAssert.IsEmpty(list);
        
        Assert.False(list.RemoveSwapBack('d'));
        CollectionAssert.IsEmpty(list);
    }

    #if !NET_DOTS
    [Test]
    public void RemoveSwapBack_Predicate()
    {
        var list = new[] { 'a', 'b', 'c', 'd' }.ToList();
        
        Assert.True(list.RemoveSwapBack(c => c == 'b'));
        CollectionAssert.AreEqual(new[] { 'a', 'd', 'c', }, list);
        
        Assert.True(list.RemoveSwapBack(c => c == 'c'));
        CollectionAssert.AreEqual(new[] { 'a', 'd' }, list);
        
        Assert.False(list.RemoveSwapBack(c => c == 'z'));
        CollectionAssert.AreEqual(new[] { 'a', 'd' }, list);
        
        Assert.True(list.RemoveSwapBack(c => c == 'a'));
        CollectionAssert.AreEqual(new[] { 'd' }, list);
        
        Assert.True(list.RemoveSwapBack(c => c == 'd'));
        CollectionAssert.IsEmpty(list);
        
        Assert.False(list.RemoveSwapBack(c => c == 'd'));
        CollectionAssert.IsEmpty(list);
    }
    #endif // !NET_DOTS
    
    [Test]
    public void RemoveAtSwapBack()
    {
        var list = new[] { 'a', 'b', 'c', 'd' }.ToList();
        
        list.RemoveAtSwapBack(1);
        CollectionAssert.AreEqual(new[] { 'a', 'd', 'c', }, list);
        
        list.RemoveAtSwapBack(2);
        CollectionAssert.AreEqual(new[] { 'a', 'd' }, list);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAtSwapBack(12));
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAtSwapBack(-5));
        
        list.RemoveAtSwapBack(0);
        CollectionAssert.AreEqual(new[] { 'd' }, list);
        
        list.RemoveAtSwapBack(0);
        CollectionAssert.IsEmpty(list);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAtSwapBack(0));
    }
}
